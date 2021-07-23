﻿// <auto-generated />
using LeagueOfItems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LeagueOfItems.ConsoleApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.5");

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Champions.Champion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Blurb")
                        .HasColumnType("TEXT");

                    b.Property<string>("Lore")
                        .HasColumnType("TEXT");

                    b.Property<int>("Matches")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("RiotId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<int>("Wins")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Champions");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Champions.ChampionData", b =>
                {
                    b.Property<int>("ChampionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rank")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Region")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Patch")
                        .HasColumnType("TEXT");

                    b.Property<int>("Matches")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Wins")
                        .HasColumnType("INTEGER");

                    b.HasKey("ChampionId", "Rank", "Region", "Role", "Patch");

                    b.ToTable("ChampionData");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Items.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Colloq")
                        .HasColumnType("TEXT");

                    b.Property<int>("Depth")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Plaintext")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Items.ItemData", b =>
                {
                    b.Property<int>("ItemId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChampionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rank")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Region")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Patch")
                        .HasColumnType("TEXT");

                    b.Property<int>("Matches")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Wins")
                        .HasColumnType("INTEGER");

                    b.HasKey("ItemId", "ChampionId", "Rank", "Order", "Region", "Role", "Patch");

                    b.HasIndex("ChampionId");

                    b.ToTable("ItemData");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Runes.Rune", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("LongDescription")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("RunePathId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ShortDescription")
                        .HasColumnType("TEXT");

                    b.Property<int>("Tier")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RunePathId");

                    b.ToTable("Runes");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Runes.RuneData", b =>
                {
                    b.Property<int>("RuneId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChampionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rank")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Tier")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Region")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Patch")
                        .HasColumnType("TEXT");

                    b.Property<int>("Matches")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Wins")
                        .HasColumnType("INTEGER");

                    b.HasKey("RuneId", "ChampionId", "Rank", "Tier", "Region", "Role", "Patch");

                    b.HasIndex("ChampionId");

                    b.ToTable("RuneData");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Runes.RunePath", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("RunePaths");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Champions.ChampionData", b =>
                {
                    b.HasOne("LeagueOfItems.Domain.Models.Champions.Champion", "Champion")
                        .WithMany("ChampionData")
                        .HasForeignKey("ChampionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Champion");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Items.ItemData", b =>
                {
                    b.HasOne("LeagueOfItems.Domain.Models.Champions.Champion", "Champion")
                        .WithMany("ItemData")
                        .HasForeignKey("ChampionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LeagueOfItems.Domain.Models.Items.Item", "Item")
                        .WithMany("ItemData")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Champion");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Runes.Rune", b =>
                {
                    b.HasOne("LeagueOfItems.Domain.Models.Runes.RunePath", "RunePath")
                        .WithMany("Runes")
                        .HasForeignKey("RunePathId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RunePath");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Runes.RuneData", b =>
                {
                    b.HasOne("LeagueOfItems.Domain.Models.Champions.Champion", "Champion")
                        .WithMany("RuneData")
                        .HasForeignKey("ChampionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LeagueOfItems.Domain.Models.Runes.Rune", "Rune")
                        .WithMany("RuneData")
                        .HasForeignKey("RuneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Champion");

                    b.Navigation("Rune");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Champions.Champion", b =>
                {
                    b.Navigation("ChampionData");

                    b.Navigation("ItemData");

                    b.Navigation("RuneData");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Items.Item", b =>
                {
                    b.Navigation("ItemData");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Runes.Rune", b =>
                {
                    b.Navigation("RuneData");
                });

            modelBuilder.Entity("LeagueOfItems.Domain.Models.Runes.RunePath", b =>
                {
                    b.Navigation("Runes");
                });
#pragma warning restore 612, 618
        }
    }
}
