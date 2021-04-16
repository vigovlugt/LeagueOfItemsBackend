using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Domain.Models;
using LeagueOfItems.Domain.Models.Ugg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace LeagueOfItems.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Item> Items { get; set; }
        DbSet<Champion> Champions { get; set; }

        DbSet<RunePath> RunePaths { get; set; }
        DbSet<Rune> Runes { get; set; }

        DbSet<UggItemData> ItemData { get; set; }
        DbSet<UggRuneData> RuneData { get; set; }

        DbSet<UggStarterSetData> StarterSetData { get; set; }
        DbSet<UggStarterSetItem> StarterSetItems { get; set; }
        
        
        Task<int> SaveChangesAsync();
        DatabaseFacade Database { get; }
    }
}