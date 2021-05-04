using System.Threading.Tasks;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Runes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LeagueOfItems.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Item> Items { get; set; }
        DbSet<Champion> Champions { get; set; }

        DbSet<RunePath> RunePaths { get; set; }
        DbSet<Rune> Runes { get; set; }

        DbSet<ItemData> ItemData { get; set; }
        DbSet<RuneData> RuneData { get; set; }
        DbSet<ChampionData> ChampionData { get; set; }
        
        DatabaseFacade Database { get; }

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        Task<int> SaveChangesAsync();
    }
}