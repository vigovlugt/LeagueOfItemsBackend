using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Runes;
using LeagueOfItems.Domain.Models.Ugg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LeagueOfItems.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Champion> Champions { get; set; }

        public DbSet<RunePath> RunePaths { get; set; }
        public DbSet<Rune> Runes { get; set; }

        public DbSet<ItemData> ItemData { get; set; }
        public DbSet<RuneData> RuneData { get; set; }
        public DbSet<ChampionData> ChampionData { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        public new DatabaseFacade Database => base.Database;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=../../data.db",
                b => b
                    .MigrationsAssembly("LeagueOfItems.ConsoleApp")
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasMany(i => i.ItemData)
                .WithOne(d => d.Item);

            modelBuilder.Entity<Rune>()
                .HasMany(i => i.RuneData)
                .WithOne(d => d.Rune);

            modelBuilder.Entity<RunePath>()
                .HasMany(p => p.Runes)
                .WithOne(b => b.RunePath)
                .IsRequired();

            modelBuilder.Entity<ItemData>()
                .HasKey(i => new {i.ItemId, i.ChampionId, i.Rank, i.Order, i.Region, i.Role, i.Patch});

            modelBuilder.Entity<RuneData>()
                .HasKey(i => new {i.RuneId, i.ChampionId, i.Rank, i.Tier, i.Region, i.Role, i.Patch});

            modelBuilder.Entity<ChampionData>()
                .HasKey(i => new {i.ChampionId, i.Rank, i.Region, i.Role, i.Patch});
        }
    }
}