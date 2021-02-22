﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using atlantis.Persistence;

namespace atlantis.Migrations
{
    [DbContext(typeof(Database))]
    partial class DatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0");

            modelBuilder.Entity("atlantis.Persistence.DbEvent", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("FactionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("TurnId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FactionId");

                    b.HasIndex("TurnId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("atlantis.Persistence.DbFaction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TurnId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TurnId");

                    b.ToTable("Factions");
                });

            modelBuilder.Entity("atlantis.Persistence.DbGame", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("EngineVersion")
                        .HasColumnType("TEXT");

                    b.Property<int>("LastTurnNumber")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("PlayerFactionName")
                        .HasColumnType("TEXT");

                    b.Property<int?>("PlayerFactionNumber")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RulesetName")
                        .HasColumnType("TEXT");

                    b.Property<string>("RulesetVersion")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("atlantis.Persistence.DbRegion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Entertainment")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Population")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Province")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Race")
                        .HasColumnType("TEXT");

                    b.Property<int>("Tax")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Terrain")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TotalWages")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TurnId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UpdatedAtTurn")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Wages")
                        .HasColumnType("REAL");

                    b.Property<int>("X")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Y")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Z")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TurnId");

                    b.ToTable("Regions");
                });

            modelBuilder.Entity("atlantis.Persistence.DbReport", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FactionName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(100);

                    b.Property<int>("FactionNumber")
                        .HasColumnType("INTEGER");

                    b.Property<long>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Json")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("TurnId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("TurnId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("atlantis.Persistence.DbTurn", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Month")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Turns");
                });

            modelBuilder.Entity("atlantis.Persistence.DbUnit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<long?>("FactionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Flags")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("OnGuard")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Orders")
                        .HasColumnType("TEXT");

                    b.Property<long>("RegionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Sequence")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TurnId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Weight")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FactionId");

                    b.HasIndex("RegionId");

                    b.HasIndex("TurnId");

                    b.ToTable("Units");
                });

            modelBuilder.Entity("atlantis.Persistence.DbEvent", b =>
                {
                    b.HasOne("atlantis.Persistence.DbFaction", "Faction")
                        .WithMany("Events")
                        .HasForeignKey("FactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("atlantis.Persistence.DbTurn", "Turn")
                        .WithMany("Events")
                        .HasForeignKey("TurnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("atlantis.Persistence.DbFaction", b =>
                {
                    b.HasOne("atlantis.Persistence.DbTurn", "Turn")
                        .WithMany("Factions")
                        .HasForeignKey("TurnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("atlantis.Persistence.DbRegion", b =>
                {
                    b.HasOne("atlantis.Persistence.DbTurn", "Turn")
                        .WithMany("Regions")
                        .HasForeignKey("TurnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("atlantis.Persistence.DbSettlement", "Settlement", b1 =>
                        {
                            b1.Property<long>("DbRegionId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Name")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Size")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.HasKey("DbRegionId");

                            b1.ToTable("Regions");

                            b1.WithOwner()
                                .HasForeignKey("DbRegionId");
                        });

                    b.OwnsMany("atlantis.Persistence.DbExit", "Exits", b1 =>
                        {
                            b1.Property<long>("RegionId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Direction")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Label")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("Province")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("Terrain")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<int>("X")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Y")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Z")
                                .HasColumnType("INTEGER");

                            b1.HasKey("RegionId", "Direction");

                            b1.ToTable("Regions_Exits");

                            b1.WithOwner()
                                .HasForeignKey("RegionId");

                            b1.OwnsOne("atlantis.Persistence.DbSettlement", "Settlement", b2 =>
                                {
                                    b2.Property<long>("DbExitRegionId")
                                        .HasColumnType("INTEGER");

                                    b2.Property<string>("DbExitDirection")
                                        .HasColumnType("TEXT");

                                    b2.Property<string>("Name")
                                        .HasColumnType("TEXT");

                                    b2.Property<string>("Size")
                                        .IsRequired()
                                        .HasColumnType("TEXT");

                                    b2.HasKey("DbExitRegionId", "DbExitDirection");

                                    b2.ToTable("Regions_Exits");

                                    b2.WithOwner()
                                        .HasForeignKey("DbExitRegionId", "DbExitDirection");
                                });
                        });

                    b.OwnsMany("atlantis.Persistence.DbItem", "Products", b1 =>
                        {
                            b1.Property<long>("RegionId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Code")
                                .HasColumnType("TEXT");

                            b1.Property<int?>("Amount")
                                .HasColumnType("INTEGER");

                            b1.HasKey("RegionId", "Code");

                            b1.ToTable("Regions_Products");

                            b1.WithOwner()
                                .HasForeignKey("RegionId");
                        });

                    b.OwnsMany("atlantis.Persistence.DbTradableItem", "ForSale", b1 =>
                        {
                            b1.Property<long>("RegionId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Code")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Amount")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Price")
                                .HasColumnType("INTEGER");

                            b1.HasKey("RegionId", "Code");

                            b1.ToTable("Regions_ForSale");

                            b1.WithOwner()
                                .HasForeignKey("RegionId");
                        });

                    b.OwnsMany("atlantis.Persistence.DbTradableItem", "Wanted", b1 =>
                        {
                            b1.Property<long>("RegionId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Code")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Amount")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Price")
                                .HasColumnType("INTEGER");

                            b1.HasKey("RegionId", "Code");

                            b1.ToTable("Regions_Wanted");

                            b1.WithOwner()
                                .HasForeignKey("RegionId");
                        });
                });

            modelBuilder.Entity("atlantis.Persistence.DbReport", b =>
                {
                    b.HasOne("atlantis.Persistence.DbGame", "Game")
                        .WithMany("Reports")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("atlantis.Persistence.DbTurn", "Turn")
                        .WithMany("Reports")
                        .HasForeignKey("TurnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("atlantis.Persistence.DbTurn", b =>
                {
                    b.HasOne("atlantis.Persistence.DbGame", "Game")
                        .WithMany("Turns")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("atlantis.Persistence.DbUnit", b =>
                {
                    b.HasOne("atlantis.Persistence.DbFaction", "Faction")
                        .WithMany("Units")
                        .HasForeignKey("FactionId");

                    b.HasOne("atlantis.Persistence.DbRegion", "Region")
                        .WithMany("Units")
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("atlantis.Persistence.DbTurn", "Turn")
                        .WithMany("Units")
                        .HasForeignKey("TurnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("atlantis.Persistence.DbCapacity", "Capacity", b1 =>
                        {
                            b1.Property<long>("DbUnitId")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Flying")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Riding")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Swimming")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Walking")
                                .HasColumnType("INTEGER");

                            b1.HasKey("DbUnitId");

                            b1.ToTable("Units");

                            b1.WithOwner()
                                .HasForeignKey("DbUnitId");
                        });

                    b.OwnsOne("atlantis.Persistence.DbItem", "ReadyItem", b1 =>
                        {
                            b1.Property<long>("DbUnitId")
                                .HasColumnType("INTEGER");

                            b1.Property<int?>("Amount")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Code")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.HasKey("DbUnitId");

                            b1.ToTable("Units");

                            b1.WithOwner()
                                .HasForeignKey("DbUnitId");
                        });

                    b.OwnsOne("atlantis.Persistence.DbSkill", "CombatSpell", b1 =>
                        {
                            b1.Property<long>("DbUnitId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Code")
                                .HasColumnType("TEXT");

                            b1.Property<int?>("Days")
                                .HasColumnType("INTEGER");

                            b1.Property<int?>("Level")
                                .HasColumnType("INTEGER");

                            b1.HasKey("DbUnitId");

                            b1.ToTable("Units");

                            b1.WithOwner()
                                .HasForeignKey("DbUnitId");
                        });

                    b.OwnsMany("atlantis.Persistence.DbItem", "Items", b1 =>
                        {
                            b1.Property<long>("UnitId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Code")
                                .HasColumnType("TEXT");

                            b1.Property<int?>("Amount")
                                .HasColumnType("INTEGER");

                            b1.HasKey("UnitId", "Code");

                            b1.ToTable("Unit_Items");

                            b1.WithOwner()
                                .HasForeignKey("UnitId");
                        });

                    b.OwnsMany("atlantis.Persistence.DbSkill", "CanStudy", b1 =>
                        {
                            b1.Property<long>("UnitId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Code")
                                .HasColumnType("TEXT");

                            b1.Property<int?>("Days")
                                .HasColumnType("INTEGER");

                            b1.Property<int?>("Level")
                                .HasColumnType("INTEGER");

                            b1.HasKey("UnitId", "Code");

                            b1.ToTable("Unit_CanStudy");

                            b1.WithOwner()
                                .HasForeignKey("UnitId");
                        });

                    b.OwnsMany("atlantis.Persistence.DbSkill", "Skills", b1 =>
                        {
                            b1.Property<long>("UnitId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Code")
                                .HasColumnType("TEXT");

                            b1.Property<int?>("Days")
                                .HasColumnType("INTEGER");

                            b1.Property<int?>("Level")
                                .HasColumnType("INTEGER");

                            b1.HasKey("UnitId", "Code");

                            b1.ToTable("Unit_Skills");

                            b1.WithOwner()
                                .HasForeignKey("UnitId");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
