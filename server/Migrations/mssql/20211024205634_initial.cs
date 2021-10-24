﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace advisor.Migrations.mssql
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Options = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ruleset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EngineVersion = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RulesetName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RulesetVersion = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Algorithm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Digest = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Roles = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Universities_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GameId = table.Column<long>(type: "bigint", nullable: false),
                    FactionNumber = table.Column<int>(type: "int", nullable: true),
                    LastTurnNumber = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsQuit = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Players_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Turns",
                columns: table => new
                {
                    Number = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turns", x => new { x.PlayerId, x.Number });
                    table.ForeignKey(
                        name: "FK_Turns_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniversityMemberships",
                columns: table => new
                {
                    AllianceId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    ShareMap = table.Column<bool>(type: "bit", nullable: false),
                    TeachMages = table.Column<bool>(type: "bit", nullable: false),
                    Owner = table.Column<bool>(type: "bit", nullable: false),
                    CanInvite = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversityMemberships", x => new { x.PlayerId, x.AllianceId });
                    table.ForeignKey(
                        name: "FK_UniversityMemberships_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniversityMemberships_Universities_AllianceId",
                        column: x => x.AllianceId,
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Factions",
                columns: table => new
                {
                    Number = table.Column<int>(type: "int", nullable: false),
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factions", x => new { x.PlayerId, x.TurnNumber, x.Number });
                    table.ForeignKey(
                        name: "FK_Factions_Turns_PlayerId_TurnNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber },
                        principalTable: "Turns",
                        principalColumns: new[] { "PlayerId", "Number" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    Z = table.Column<int>(type: "int", nullable: false),
                    Explored = table.Column<bool>(type: "bit", nullable: false),
                    LastVisitedAt = table.Column<int>(type: "int", nullable: true),
                    Label = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Terrain = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Settlement_Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Settlement_Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Population = table.Column<int>(type: "int", nullable: false),
                    Race = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Entertainment = table.Column<int>(type: "int", nullable: false),
                    Tax = table.Column<int>(type: "int", nullable: false),
                    Wages = table.Column<double>(type: "float", nullable: false),
                    TotalWages = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => new { x.PlayerId, x.TurnNumber, x.Id });
                    table.ForeignKey(
                        name: "FK_Regions_Turns_PlayerId_TurnNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber },
                        principalTable: "Turns",
                        principalColumns: new[] { "PlayerId", "Number" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    FactionNumber = table.Column<int>(type: "int", nullable: false),
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    FactionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Json = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => new { x.PlayerId, x.TurnNumber, x.FactionNumber });
                    table.ForeignKey(
                        name: "FK_Reports_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_Turns_PlayerId_TurnNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber },
                        principalTable: "Turns",
                        principalColumns: new[] { "PlayerId", "Number" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Exits",
                columns: table => new
                {
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    OriginRegionId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    TargetRegionId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exits", x => new { x.PlayerId, x.TurnNumber, x.OriginRegionId, x.TargetRegionId });
                    table.ForeignKey(
                        name: "FK_Exits_Regions_PlayerId_TurnNumber_OriginRegionId",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.OriginRegionId },
                        principalTable: "Regions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exits_Regions_PlayerId_TurnNumber_TargetRegionId",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.TargetRegionId },
                        principalTable: "Regions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exits_Turns_PlayerId_TurnNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber },
                        principalTable: "Turns",
                        principalColumns: new[] { "PlayerId", "Number" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionItems",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionItems", x => new { x.PlayerId, x.TurnNumber, x.RegionId, x.Code });
                    table.ForeignKey(
                        name: "FK_ProductionItems_Regions_PlayerId_TurnNumber_RegionId",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.RegionId },
                        principalTable: "Regions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductionItems_Turns_PlayerId_TurnNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber },
                        principalTable: "Turns",
                        principalColumns: new[] { "PlayerId", "Number" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    FactionNumber = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Income_Work = table.Column<int>(type: "int", nullable: true),
                    Income_Tax = table.Column<int>(type: "int", nullable: true),
                    Income_Pillage = table.Column<int>(type: "int", nullable: true),
                    Income_Trade = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => new { x.PlayerId, x.TurnNumber, x.FactionNumber, x.RegionId });
                    table.ForeignKey(
                        name: "FK_Stats_Factions_PlayerId_TurnNumber_FactionNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.FactionNumber },
                        principalTable: "Factions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Number" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stats_Regions_PlayerId_TurnNumber_RegionId",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.RegionId },
                        principalTable: "Regions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stats_Turns_PlayerId_TurnNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber },
                        principalTable: "Turns",
                        principalColumns: new[] { "PlayerId", "Number" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Structures",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Contents = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SailDirections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Speed = table.Column<int>(type: "int", nullable: true),
                    Needs = table.Column<int>(type: "int", nullable: true),
                    Load_Used = table.Column<int>(type: "int", nullable: true),
                    Load_Max = table.Column<int>(type: "int", nullable: true),
                    Sailors_Current = table.Column<int>(type: "int", nullable: true),
                    Sailors_Required = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Structures", x => new { x.PlayerId, x.TurnNumber, x.Id });
                    table.ForeignKey(
                        name: "FK_Structures_Regions_PlayerId_TurnNumber_RegionId",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.RegionId },
                        principalTable: "Regions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Structures_Turns_PlayerId_TurnNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber },
                        principalTable: "Turns",
                        principalColumns: new[] { "PlayerId", "Number" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TradeItems",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Market = table.Column<int>(type: "int", nullable: false),
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    DbRegionId = table.Column<string>(type: "nvarchar(14)", nullable: true),
                    DbRegionId1 = table.Column<string>(type: "nvarchar(14)", nullable: true),
                    DbRegionPlayerId = table.Column<long>(type: "bigint", nullable: true),
                    DbRegionPlayerId1 = table.Column<long>(type: "bigint", nullable: true),
                    DbRegionTurnNumber = table.Column<int>(type: "int", nullable: true),
                    DbRegionTurnNumber1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeItems", x => new { x.PlayerId, x.TurnNumber, x.RegionId, x.Market, x.Code });
                    table.ForeignKey(
                        name: "FK_TradeItems_Regions_DbRegionPlayerId_DbRegionTurnNumber_DbRegionId",
                        columns: x => new { x.DbRegionPlayerId, x.DbRegionTurnNumber, x.DbRegionId },
                        principalTable: "Regions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeItems_Regions_DbRegionPlayerId1_DbRegionTurnNumber1_DbRegionId1",
                        columns: x => new { x.DbRegionPlayerId1, x.DbRegionTurnNumber1, x.DbRegionId1 },
                        principalTable: "Regions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeItems_Regions_PlayerId_TurnNumber_RegionId",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.RegionId },
                        principalTable: "Regions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradeItems_Turns_PlayerId_TurnNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber },
                        principalTable: "Turns",
                        principalColumns: new[] { "PlayerId", "Number" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatItems",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    FactionNumber = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    RegionPlayerId = table.Column<long>(type: "bigint", nullable: true),
                    RegionTurnNumber = table.Column<int>(type: "int", nullable: true),
                    RegionId1 = table.Column<string>(type: "nvarchar(14)", nullable: true),
                    FactionPlayerId = table.Column<long>(type: "bigint", nullable: true),
                    FactionTurnNumber = table.Column<int>(type: "int", nullable: true),
                    FactionNumber1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatItems", x => new { x.PlayerId, x.TurnNumber, x.FactionNumber, x.RegionId, x.Code });
                    table.ForeignKey(
                        name: "FK_StatItems_Factions_FactionPlayerId_FactionTurnNumber_FactionNumber1",
                        columns: x => new { x.FactionPlayerId, x.FactionTurnNumber, x.FactionNumber1 },
                        principalTable: "Factions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Number" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatItems_Regions_RegionPlayerId_RegionTurnNumber_RegionId1",
                        columns: x => new { x.RegionPlayerId, x.RegionTurnNumber, x.RegionId1 },
                        principalTable: "Regions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatItems_Stats_PlayerId_TurnNumber_FactionNumber_RegionId",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.FactionNumber, x.RegionId },
                        principalTable: "Stats",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "FactionNumber", "RegionId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Number = table.Column<int>(type: "int", nullable: false),
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    StrcutureId = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: true),
                    FactionNumber = table.Column<int>(type: "int", nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    OnGuard = table.Column<bool>(type: "bit", nullable: false),
                    Flags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<int>(type: "int", nullable: true),
                    Capacity_Flying = table.Column<int>(type: "int", nullable: true),
                    Capacity_Riding = table.Column<int>(type: "int", nullable: true),
                    Capacity_Walking = table.Column<int>(type: "int", nullable: true),
                    Capacity_Swimming = table.Column<int>(type: "int", nullable: true),
                    Skills = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanStudy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReadyItem = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    CombatSpell = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Orders = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => new { x.PlayerId, x.TurnNumber, x.Number });
                    table.ForeignKey(
                        name: "FK_Units_Factions_PlayerId_TurnNumber_FactionNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.FactionNumber },
                        principalTable: "Factions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Number" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Units_Regions_PlayerId_TurnNumber_RegionId",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.RegionId },
                        principalTable: "Regions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Units_Structures_PlayerId_TurnNumber_StrcutureId",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.StrcutureId },
                        principalTable: "Structures",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Units_Turns_PlayerId_TurnNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber },
                        principalTable: "Turns",
                        principalColumns: new[] { "PlayerId", "Number" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    FactionNumber = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    UnitNumber = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ItemPrice = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Factions_PlayerId_TurnNumber_FactionNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.FactionNumber },
                        principalTable: "Factions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Number" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_Regions_PlayerId_TurnNumber_RegionId",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.RegionId },
                        principalTable: "Regions",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Turns_PlayerId_TurnNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber },
                        principalTable: "Turns",
                        principalColumns: new[] { "PlayerId", "Number" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Units_PlayerId_TurnNumber_UnitNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.UnitNumber },
                        principalTable: "Units",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Number" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    UnitNumber = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => new { x.PlayerId, x.TurnNumber, x.UnitNumber, x.Code });
                    table.ForeignKey(
                        name: "FK_Items_Turns_PlayerId_TurnNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber },
                        principalTable: "Turns",
                        principalColumns: new[] { "PlayerId", "Number" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Units_PlayerId_TurnNumber_UnitNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.UnitNumber },
                        principalTable: "Units",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Number" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyPlans",
                columns: table => new
                {
                    UnitNumber = table.Column<int>(type: "int", nullable: false),
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    Target_Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Target_Level = table.Column<int>(type: "int", nullable: true),
                    Study = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Teach = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlans", x => new { x.PlayerId, x.TurnNumber, x.UnitNumber });
                    table.ForeignKey(
                        name: "FK_StudyPlans_Turns_PlayerId_TurnNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber },
                        principalTable: "Turns",
                        principalColumns: new[] { "PlayerId", "Number" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudyPlans_Units_PlayerId_TurnNumber_UnitNumber",
                        columns: x => new { x.PlayerId, x.TurnNumber, x.UnitNumber },
                        principalTable: "Units",
                        principalColumns: new[] { "PlayerId", "TurnNumber", "Number" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_PlayerId_TurnNumber_FactionNumber",
                table: "Events",
                columns: new[] { "PlayerId", "TurnNumber", "FactionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_PlayerId_TurnNumber_RegionId",
                table: "Events",
                columns: new[] { "PlayerId", "TurnNumber", "RegionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_PlayerId_TurnNumber_UnitNumber",
                table: "Events",
                columns: new[] { "PlayerId", "TurnNumber", "UnitNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Exits_PlayerId_TurnNumber_TargetRegionId",
                table: "Exits",
                columns: new[] { "PlayerId", "TurnNumber", "TargetRegionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameId",
                table: "Players",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId",
                table: "Players",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StatItems_FactionPlayerId_FactionTurnNumber_FactionNumber1",
                table: "StatItems",
                columns: new[] { "FactionPlayerId", "FactionTurnNumber", "FactionNumber1" });

            migrationBuilder.CreateIndex(
                name: "IX_StatItems_RegionPlayerId_RegionTurnNumber_RegionId1",
                table: "StatItems",
                columns: new[] { "RegionPlayerId", "RegionTurnNumber", "RegionId1" });

            migrationBuilder.CreateIndex(
                name: "IX_Stats_PlayerId_TurnNumber_RegionId",
                table: "Stats",
                columns: new[] { "PlayerId", "TurnNumber", "RegionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Structures_PlayerId_TurnNumber_RegionId",
                table: "Structures",
                columns: new[] { "PlayerId", "TurnNumber", "RegionId" });

            migrationBuilder.CreateIndex(
                name: "IX_TradeItems_DbRegionPlayerId_DbRegionTurnNumber_DbRegionId",
                table: "TradeItems",
                columns: new[] { "DbRegionPlayerId", "DbRegionTurnNumber", "DbRegionId" });

            migrationBuilder.CreateIndex(
                name: "IX_TradeItems_DbRegionPlayerId1_DbRegionTurnNumber1_DbRegionId1",
                table: "TradeItems",
                columns: new[] { "DbRegionPlayerId1", "DbRegionTurnNumber1", "DbRegionId1" });

            migrationBuilder.CreateIndex(
                name: "IX_Units_PlayerId_TurnNumber_FactionNumber",
                table: "Units",
                columns: new[] { "PlayerId", "TurnNumber", "FactionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Units_PlayerId_TurnNumber_RegionId",
                table: "Units",
                columns: new[] { "PlayerId", "TurnNumber", "RegionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Units_PlayerId_TurnNumber_StrcutureId",
                table: "Units",
                columns: new[] { "PlayerId", "TurnNumber", "StrcutureId" });

            migrationBuilder.CreateIndex(
                name: "IX_Universities_GameId",
                table: "Universities",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_UniversityMemberships_AllianceId",
                table: "UniversityMemberships",
                column: "AllianceId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Exits");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "ProductionItems");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "StatItems");

            migrationBuilder.DropTable(
                name: "StudyPlans");

            migrationBuilder.DropTable(
                name: "TradeItems");

            migrationBuilder.DropTable(
                name: "UniversityMemberships");

            migrationBuilder.DropTable(
                name: "Stats");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Universities");

            migrationBuilder.DropTable(
                name: "Factions");

            migrationBuilder.DropTable(
                name: "Structures");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "Turns");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
