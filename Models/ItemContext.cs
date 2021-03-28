using LeagueOfItems.Models.Ugg;
using Microsoft.EntityFrameworkCore;

namespace LeagueOfItems.Models
{
    public class ItemContext : DbContext
    {
        public ItemContext(DbContextOptions<ItemContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=data.db");
        }
        
        public DbSet<Item> Items { get; set; }
        public DbSet<Champion> Champions { get; set; }
        
        public DbSet<RunePath> RunePaths { get; set; }
        public DbSet<Rune> Runes { get; set; }
        
        public DbSet<UggItemData> ItemData { get; set; }
        public DbSet<UggRuneData> RuneData { get; set; }
        
        public DbSet<UggStarterSetData> StarterSetData { get; set; }
        public DbSet<UggStarterSetItem> StarterSetItems { get; set; }
        
        
        
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
            
            modelBuilder.Entity<UggItemData>()
                .HasKey(i => new {i.ItemId, i.ChampionId, i.Rank, i.Order, i.Region, i.Role} );
            
            modelBuilder.Entity<UggRuneData>()
                .HasKey(i => new {i.RuneId, i.ChampionId, i.Rank, i.Tier, i.Region, i.Role} );
            
            modelBuilder.Entity<UggStarterSetData>()
                .HasMany(p => p.Items)
                .WithOne(b => b.UggStarterSet)
                .IsRequired();

            modelBuilder.Entity<UggStarterSetItem>()
                .HasKey(i => new {i.StarterSetId, i.ItemId});
            
            
        }
    }
}