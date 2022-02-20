﻿namespace advisor {
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;
    using Microsoft.EntityFrameworkCore;
    using Persistence;


    public class TurnType : ObjectType<DbTurn> {
        protected override void Configure(IObjectTypeDescriptor<DbTurn> descriptor) {
            descriptor
                .ImplementsNode()
                .IdField(x => x.CompsiteId)
                .ResolveNode((ctx, id) => {
                    var db = ctx.Service<Database>();
                    var parsedId = DbTurn.ParseId(id);
                    return DbTurn.FilterById(db.Turns.AsNoTracking(), parsedId)
                        .Include(x => x.Player)
                        .SingleOrDefaultAsync();
                });
        }
    }

    [ExtendObjectType("Turn")]
    public class TurnResolvers {
        public Task<List<DbReport>> GetReports(Database db, [Parent] DbTurn turn) {
            return db.Reports
                .AsNoTracking()
                .FilterByTurn(turn)
                .OrderBy(x => x.FactionNumber)
                .ToListAsync();
        }

        [UseOffsetPaging(IncludeTotalCount = true, MaxPageSize = 1000)]
        public IQueryable<DbRegion> GetRegions(Database db, [Parent] DbTurn turn, bool withStructures = false) {
            IQueryable<DbRegion> query = db.Regions
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .FilterByTurn(turn)
                .Include(x => x.Exits)
                .Include(x => x.Produces)
                .Include(x => x.Markets);

            if (withStructures) {
                query = query.Include(x => x.Structures);
            }

            return query.OrderBy(x => x.Id);
        }

        [UseOffsetPaging(IncludeTotalCount = true, MaxPageSize = 1000)]
        public IQueryable<DbStructure> Structures(Database db, [Parent] DbTurn turn) {
            return db.Structures
                .AsNoTracking()
                .FilterByTurn(turn)
                .OrderBy(x => x.RegionId)
                .ThenBy(x => x.Sequence);
        }

        [UseOffsetPaging(IncludeTotalCount = true, MaxPageSize = 1000)]
        public async Task<IQueryable<DbUnit>> Units(IResolverContext context, Database db, [Parent] DbTurn turn, UnitsFilter filter = null) {
            var fields = context.CollectSelectedFields<DbUnit>();

            var query = db.Units
                .AsNoTrackingWithIdentityResolution()
                .FilterByTurn(turn);

            if (fields.Contains(nameof(DbUnit.Items))) {
                query = query.Include(x => x.Items);
            }

            if (fields.Contains(nameof(DbUnit.StudyPlan))) {
                query = query.Include(x => x.StudyPlan);
            }

            if (filter != null) {
                if (filter.Own != null) {
                    var factionNumber = turn.Player.Number;

                    query = filter.Own.Value
                        ? query.Where(x => x.FactionNumber == factionNumber)
                        : query.Where(x => x.FactionNumber != factionNumber);
                }

                if (filter.Mages != null) {
                    query = (await query.ToListAsync())
                        .Where(x => x.Skills.Any(s => s.Code == "FORC" || s.Code == "PATT" || s.Code == "SPIR"))
                        .AsQueryable();
                }
            }

            return query.OrderBy(x => x.Number);
        }

        [UseOffsetPaging(IncludeTotalCount = true, MaxPageSize = 1000)]
        public IQueryable<DbEvent> Events(Database db, [Parent] DbTurn turn) {
            return db.Events
                .AsNoTracking()
                .FilterByTurn(turn)
                .OrderBy(x => x.Id);
        }

        public Task<List<DbFaction>> Factions(Database db, [Parent] DbTurn turn) {
            return db.Factions
                .AsNoTrackingWithIdentityResolution()
                .Include(x => x.Attitudes)
                .OrderBy(x => x.Number)
                .FilterByTurn(turn)
                .ToListAsync();
        }

        public async Task<Statistics> Stats(Database db, [Parent] DbTurn turn) {
            var factionNumber = turn.Player.Number;
            var stats = await db.Stats
                .AsNoTracking()
                .FilterByTurn(turn)
                .Include(x => x.Production)
                .ToListAsync();

            DbIncomeStats income = new DbIncomeStats();
            Dictionary<string, int> production = new Dictionary<string, int>();

            foreach (var stat in stats) {
                income.Pillage += stat.Income.Pillage;
                income.Tax += stat.Income.Tax;
                income.Trade += stat.Income.Trade;
                income.Work += stat.Income.Work;

                foreach (var item in stat.Production) {
                    production[item.Code] = production.TryGetValue(item.Code, out var value)
                        ? value + item.Amount
                        : item.Amount;
                }
            }

            return new Statistics {
                Income = income,
                Production = production.Select(x => new Item { Code = x.Key, Amount = x.Value }).ToList()
            };
        }

        public Task<List<DbStudyPlan>> StudyPlans(Database db, [Parent] DbTurn turn) {
            return db.StudyPlans
                .AsNoTrackingWithIdentityResolution()
                .FilterByTurn(turn)
                .ToListAsync();
        }
    }
}
