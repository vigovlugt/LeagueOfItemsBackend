﻿// <auto-generated />
using LeagueOfItems.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LeagueOfItems.Migrations
{
    [DbContext(typeof(ItemContext))]
    partial class ItemContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.4");

            modelBuilder.Entity("LeagueOfItems.Models.Champion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Blurb")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("RiotId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Champions");
                });

            modelBuilder.Entity("LeagueOfItems.Models.Item", b =>
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

            modelBuilder.Entity("LeagueOfItems.Models.ItemData", b =>
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

                    b.Property<int>("Matches")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Wins")
                        .HasColumnType("INTEGER");

                    b.HasKey("ItemId", "ChampionId", "Rank", "Order", "Region", "Role");

                    b.ToTable("ItemData");
                });

            modelBuilder.Entity("LeagueOfItems.Models.StarterSetData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChampionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Matches")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rank")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Region")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Wins")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("StarterSetData");
                });

            modelBuilder.Entity("LeagueOfItems.Models.StarterSetItem", b =>
                {
                    b.Property<int>("StarterSetId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.HasKey("StarterSetId", "ItemId");

                    b.ToTable("StarterSetItems");
                });

            modelBuilder.Entity("LeagueOfItems.Models.StarterSetItem", b =>
                {
                    b.HasOne("LeagueOfItems.Models.StarterSetData", "StarterSet")
                        .WithMany("Items")
                        .HasForeignKey("StarterSetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StarterSet");
                });

            modelBuilder.Entity("LeagueOfItems.Models.StarterSetData", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
