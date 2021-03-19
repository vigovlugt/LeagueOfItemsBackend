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
        
        public DbSet<ItemData> ItemData { get; set; }
        
        public DbSet<StarterSetData> StarterSetData { get; set; }
        public DbSet<StarterSetItem> StarterSetItems { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasMany(i => i.ItemData)
                .WithOne(d => d.Item);
            
            modelBuilder.Entity<ItemData>()
                .HasKey(i => new {i.ItemId, i.ChampionId, i.Rank, i.Order, i.Region, i.Role} );
            
            modelBuilder.Entity<StarterSetData>()
                .HasMany(p => p.Items)
                .WithOne(b => b.StarterSet)
                .IsRequired();

            modelBuilder.Entity<StarterSetItem>()
                .HasKey(i => new {i.StarterSetId, i.ItemId});
        }
    }
}