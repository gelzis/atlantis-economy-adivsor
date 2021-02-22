namespace atlantis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using atlantis.Persistence;
    using HotChocolate.Types.Relay;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Model;

    using RegionDic = System.Collections.Generic.Dictionary<string, Persistence.DbRegion>;
    using FactionsDic = System.Collections.Generic.Dictionary<int, Persistence.DbFaction>;
    using UnitsDic = System.Collections.Generic.Dictionary<int, Persistence.DbUnit>;

    [Route("report")]
    public class ApiController : ControllerBase {
        public ApiController(Database db, IIdSerializer relayId) {
            this.db = db;
            this.relayId = relayId;
        }

        private readonly Database db;
        private readonly IIdSerializer relayId;

        [HttpPost("{gameId}")]
        public async Task<IActionResult> UploadReports([Required, FromRoute] string gameId) {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (Request.Form.Files.Count == 0) return UnprocessableEntity();

            DbGame game = await FindGameAsync(db, relayId, gameId);
            if (game == null) return NotFound();

            // 1. upload reports
            int earliestTurn = int.MaxValue;
            foreach (var file in Request.Form.Files) {
                await using var stream = file.OpenReadStream();
                var turnNumber = await LoadReportFileAsync(db, stream, game);

                earliestTurn = Math.Min(earliestTurn, turnNumber);
            }
            await db.SaveChangesAsync();

            // 2. parse & merge reports and update history
            await UpdateTurnsAsync(db, game.Id, earliestTurn);

            return Ok();
        }

        public const string DEFAULT_LEVEL_LABEL = "surface";
        public const int DEFAULT_LEVEL_Z = 1;

        private static Task<DbGame> FindGameAsync(Database db, IIdSerializer relayId, string gameId) {
            var id = (long) relayId.Deserialize(gameId).Value;

            return db.Games.SingleOrDefaultAsync(x => x.Id == id);
        }

        private static async Task<int> LoadReportFileAsync(Database db, Stream reportStream, DbGame game) {
            using var textReader = new StreamReader(reportStream);

            using var atlantisReader = new AtlantisReportJsonConverter(textReader);
            var json = await atlantisReader.ReadAsJsonAsync();

            var faction = json["faction"].ToObject<JFaction>();
            var date = json["date"].ToObject<JDate>();
            var engine = json["engine"]?.ToObject<JGameEngine>();
            var ordersTemplate = json["ordersTemplate"]?.ToObject<JOrdersTemplate>();

            string factionName = faction.Name;
            int factionNumber = faction.Number;
            int year = date.Year;
            int month = date.Month;
            int turnNumber = month + (year - 1) * 12;

            reportStream.Seek(0, SeekOrigin.Begin);
            var source = await textReader.ReadToEndAsync();

            var turn = await db.Turns
                .FirstOrDefaultAsync(x => x.GameId == game.Id && x.Number == turnNumber);
            DbReport report = null;

            if (turn == null) {
                turn = new DbTurn {
                    GameId = game.Id,
                    Month = month,
                    Year = year,
                    Number = turnNumber
                };

                game.Turns.Add(turn);
            }
            else {
                report = await db.Reports
                    .FirstOrDefaultAsync(x => x.FactionNumber == factionNumber && x.TurnId == turn.Id);
            }

            if (report == null) {
                report = new DbReport {
                    GameId = game.Id,
                    TurnId = turn.Id
                };

                game.Reports.Add(report);
                turn.Reports.Add(report);
            }

            report.FactionNumber = factionNumber;
            report.FactionName = factionName;
            report.Source = source;
            report.Json = json.ToString(Formatting.Indented);

            game.PlayerFactionNumber ??= report.FactionNumber;
            if (report.FactionNumber == game.PlayerFactionNumber && turnNumber > game.LastTurnNumber) {
                game.PlayerFactionName = report.FactionName;
                game.LastTurnNumber = turnNumber;
            }

            game.EngineVersion ??= engine?.Version;
            game.RulesetName ??= engine?.Ruleset?.Name;
            game.RulesetVersion ??= engine?.Ruleset?.Version;
            game.Password ??= ordersTemplate?.Password;

            return turnNumber;
        }

        private static async Task UpdateTurnsAsync(Database db, long gameId, int earliestTurn) {
            var lastTurn = await db.Turns
                .AsNoTracking()
                .OrderByDescending(x => x.Number)
                .Where(x => x.GameId == gameId)
                .Select(x => x.Number)
                .FirstOrDefaultAsync();

            if (lastTurn > earliestTurn) {
                lastTurn = earliestTurn - 1;
            }

            var turns = await db.Turns
                .AsNoTracking()
                .Where(x => x.GameId == gameId && x.Number >= lastTurn)
                .OrderBy(x => x.Number)
                .Select(x => new { x.Id, x.Number })
                .ToListAsync();

            RegionDic regions = new RegionDic();
            FactionsDic factions = new FactionsDic();
            UnitsDic units = new UnitsDic();

            if (earliestTurn > 1) {
                regions = (await db.Regions
                        .AsNoTracking()
                        .Where(x => x.TurnId == turns[0].Id)
                        .ToArrayAsync())
                    .ToDictionary(k => k.UID);

                factions = (await db.Factions
                        .AsNoTracking()
                        .Where(x => x.TurnId == turns[0].Id)
                        .ToArrayAsync())
                    .ToDictionary(k => k.Number);
            }

            foreach (var turn in turns) {
                await MergeReportsAsync(db, turn.Id, turn.Number, regions, factions, units);

                await RemoveEventsAsync(db, turn.Id);
                await RemoveUnitsAsync(db, turn.Id);
                await RemoveFactionsAsync(db, turn.Id);
                await RemoveRegionsAsync(db, turn.Id);

                await InsertFactionsAsync(db, turn.Id, factions.Values);
                await InsertRegionsAsync(db, turn.Id, regions.Values);

                await db.SaveChangesAsync();
            }
        }

        // This method will be memory intensive as it must merge all reports into one
        private static async Task MergeReportsAsync(Database db, long turnId, int turnNumber, RegionDic regions, FactionsDic factions, UnitsDic units) {
            var savedReports = db.Reports
                .Where(x => x.TurnId == turnId)
                .AsNoTracking()
                .AsAsyncEnumerable();

            int i = 0;
            await foreach (var r in savedReports) {
                var report = JsonConvert.DeserializeObject<JReport>(r.Json);

                foreach (var region in report.Regions) {
                    CreateOrUpdateRegion(turnNumber, factions, regions, units, region);
                }

                CreateOrUpdateFaction(factions, report);
            }

            AddRevealedRegionsFromExits(turnNumber, regions);
        }

        public static DbFaction CreateOrUpdateFaction(FactionsDic factions, JFaction f) {
            if (!factions.TryGetValue(f.Number, out var faction)) {
                faction = new DbFaction {
                    Number = f.Number
                };
                factions.Add(faction.Number, faction);
            }

            faction.Name = f.Name;

            return faction;
        }

        public static DbFaction CreateOrUpdateFaction(FactionsDic factions, JReport report) {
            var f = report.Faction;

            var faction = CreateOrUpdateFaction(factions, f);

            foreach (var error in report.Errors) {
                faction.Events.Add(new DbEvent {
                    Type = EventType.Error,
                    Message = error
                });
            }

            foreach (var ev in report.Events) {
                faction.Events.Add(new DbEvent {
                    Type = EventType.Info,
                    Message = ev
                });
            }

            return faction;
        }

        private static DbRegion CreateOrUpdateRegion(int turnNumber, FactionsDic factions, RegionDic regions, UnitsDic units, JRegion source) {
            var x = source.Coords.X;
            var y = source.Coords.Y;
            var z = source.Coords.Z ?? DEFAULT_LEVEL_Z;

            var uid = DbRegion.GetUID(x, y, z);

            DbRegion region;
            if (!regions.TryGetValue(uid, out region)) {
                region = new DbRegion();
                regions.Add(uid, region);
            }

            region.UpdatedAtTurn = turnNumber;
            region.X = x;
            region.Y = y;
            region.Z = z;
            region.Label = source.Coords.Label ?? DEFAULT_LEVEL_LABEL;
            region.Province = source.Province;
            region.Terrain = source.Terrain;

            if (source.Settlement != null) {
                if (region.Settlement == null) {
                    region.Settlement = new DbSettlement();
                }

                region.Settlement.Name = source.Settlement.Name;
                region.Settlement.Size = source.Settlement.Size;
            }
            else {
                region.Settlement = null;
            }

            region.Population = source.Population?.Amount ?? 0;
            region.Race = source.Population?.Race;
            region.Tax = source.Tax ?? 0;
            region.Entertainment = source.Entertainment ?? 0;
            region.Wages = source.Wages;
            region.TotalWages = source.TotalWages ?? 0;

            SetItems(region.Products, source.Products);
            SetItems(region.Wanted, source.Wanted);
            SetItems(region.ForSale, source.ForSale);
            SetExits(region.Exits, source.Exits);

            int seq = 0;
            foreach (var unit in source.Units) {
                CreateOrUpdateUnit(factions, units, region, unit, seq++);
            }

            return region;
        }

        private static DbUnit CreateOrUpdateUnit(FactionsDic factions, UnitsDic units, DbRegion region, JUnit source, int seq) {
            if (!units.TryGetValue(source.Number, out var unit)) {
                unit = new DbUnit {
                    Number = source.Number
                };

                units.Add(unit.Number, unit);
                region.Units.Add(unit);
            }

            if (source.Faction != null && unit.Faction == null) {
                var faction = CreateOrUpdateFaction(factions, source.Faction);
                faction.Units.Add(unit);
                unit.Faction = faction;
            }

            unit.Sequence = seq;
            unit.Name = source.Name;
            unit.Description = source.Description;
            unit.OnGuard = source.OnGuard;
            unit.Flags = unit.Flags.Union(source.Flags).ToList();
            unit.Weight = source.Weight;

            if (source.Capacity != null) {
                unit.Capacity = new DbCapacity {
                    Walking = source.Capacity.Walking,
                    Swimming = source.Capacity.Swimming,
                    Riding = source.Capacity.Riding,
                    Flying = source.Capacity.Flying
                };
            }

            if (source.ReadyItem != null) {
                unit.ReadyItem = new DbItem {
                    Code = source.ReadyItem.Code,
                    Amount = source.ReadyItem.Amount
                };
            }

            if (source.CombatSpell != null) {
                unit.CombatSpell = new DbSkill {
                    Code = source.CombatSpell.Code,
                    Level = source.CombatSpell.Level,
                    Days = source.CombatSpell.Days
                };
            }

            SetItems(unit.Items, source.Items);
            SetSkills(unit.Skills, source.Skills);
            SetSkills(unit.CanStudy, source.CanStudy);

            return unit;
        }

        private static Task RemoveRegionsAsync(Database db, long turnId) {
            return db.Database
                .ExecuteSqlRawAsync($"delete from {db.Model.FindEntityType(typeof(DbRegion)).GetTableName()} where TurnId = {turnId}");
        }

        private static Task RemoveFactionsAsync(Database db, long turnId) {
            return db.Database
                .ExecuteSqlRawAsync($"delete from {db.Model.FindEntityType(typeof(DbFaction)).GetTableName()} where TurnId = {turnId}");
        }

        private static Task RemoveEventsAsync(Database db, long turnId) {
            return db.Database
                .ExecuteSqlRawAsync($"delete from {db.Model.FindEntityType(typeof(DbEvent)).GetTableName()} where TurnId = {turnId}");
        }

        private static Task RemoveUnitsAsync(Database db, long turnId) {
            return db.Database
                .ExecuteSqlRawAsync($"delete from {db.Model.FindEntityType(typeof(DbUnit)).GetTableName()} where TurnId = {turnId}");
        }

        private static async Task InsertRegionsAsync(Database db, long turnId, IEnumerable<DbRegion> regions) {
            foreach (var region in regions) {
                region.TurnId = turnId;

                foreach (var unit in region.Units) {
                    unit.TurnId = turnId;
                }

                await db.AddAsync(region);
            }
        }

        private static async Task InsertFactionsAsync(Database db, long turnId, IEnumerable<DbFaction> factions) {
            foreach (var faction in factions) {
                faction.TurnId = turnId;

                foreach (var ev in faction.Events) {
                    ev.TurnId = turnId;
                }

                await db.AddAsync(faction);
            }
        }

        public static void SetItems(ICollection<DbItem> dbItems, IEnumerable<JItem> items) {
            var dest = dbItems.ToDictionary(x => x.Code);

            foreach (var item in items) {
                if (!dest.TryGetValue(item.Code, out var d)) {
                    d = new DbItem();
                    dbItems.Add(d);
                }

                d.Code = item.Code;
                d.Amount = item.Amount;
            }
        }

        public static void SetSkills(ICollection<DbSkill> dbItems, IEnumerable<JSkill> items) {
            var dest = dbItems.ToDictionary(x => x.Code);

            foreach (var item in items) {
                if (!dest.TryGetValue(item.Code, out var d)) {
                    d = new DbSkill();
                    dbItems.Add(d);
                }

                d.Code = item.Code;
                d.Level = item.Level;
                d.Days = item.Days;
            }
        }

        public static void SetItems(ICollection<DbTradableItem> dbItems, IEnumerable<JTradableItem> items) {
            var dest = dbItems.ToDictionary(x => x.Code);

            foreach (var item in items) {
                if (!dest.TryGetValue(item.Code, out var d)) {
                    d = new DbTradableItem();
                    dbItems.Add(d);
                }

                d.Code = item.Code;
                d.Amount = item.Amount ?? 1;
                d.Price = item.Price;
            }
        }

        public static void SetExits(ICollection<DbExit> dbItems, IEnumerable<JExit> items) {
            var dest = dbItems.ToDictionary(x => x.RegionUID);
            foreach (var item in items) {
                var uid = DbRegion.GetUID(item.Coords.X, item.Coords.Y, item.Coords.Z ?? 1);
                if (!dest.TryGetValue(uid, out var d)) {
                    d = new DbExit();
                    dbItems.Add(d);
                }

                d.Direction = item.Direction;
                d.X = item.Coords.X;
                d.Y = item.Coords.Y;
                d.Z = item.Coords.Z ?? DEFAULT_LEVEL_Z;
                d.Label = item.Coords.Label ?? DEFAULT_LEVEL_LABEL;
                d.Province = item.Province;
                d.Terrain = item.Terrain;

                if (item.Settlement != null) {
                    if (d.Settlement == null) {
                        d.Settlement = new DbSettlement();
                    }

                    d.Settlement.Name = item.Settlement.Name;
                    d.Settlement.Size = item.Settlement.Size;
                }
                else {
                    d.Settlement = null;
                }
            }
        }

        public static void AddRevealedRegionsFromExits(int turnNumber, RegionDic regions) {
            var exits = regions.Values
                .SelectMany(x => x.Exits)
                .GroupBy(x => x.RegionUID)
                .ToDictionary(x => x.Key, x => x.First());

            foreach (var uid in exits.Keys.Except(regions.Keys)) {
                var exit = exits[uid];

                DbRegion region = new DbRegion();
                regions.Add(uid, region);

                region.UpdatedAtTurn = turnNumber;
                region.X = exit.X;
                region.Y = exit.Y;
                region.Z = exit.Z;
                region.Label = exit.Label;
                region.Province = exit.Province;
                region.Terrain = exit.Terrain;
            }
        }
    }
}
