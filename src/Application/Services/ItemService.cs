using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeagueOfItems.Application.Contexts;
using LeagueOfItems.Domain.Models;
using LeagueOfItems.Domain.Models.Filters;
using LeagueOfItems.Domain.Models.Ugg;
using Microsoft.EntityFrameworkCore;

namespace LeagueOfItems.Application.Services
{
    public interface IItemService
    {
        Task<List<ItemStats>> GetAllItems(ItemFilter filter);
        Task<ItemStats> GetItemStats(int id, ItemFilter filter);
    }

    public class ItemService : IItemService
    {
        private readonly ItemContext _context;

        public ItemService(ItemContext context)
        {
            _context = context;
        }

        public async Task<List<ItemStats>> GetAllItems(ItemFilter filter)
        {
            var items = await _context.Items
                .Include(i => i.ItemData.Where(d =>
                    d.Rank == filter.Rank && d.Region == filter.Region &&
                    (d.Role == filter.Role || filter.Role == UggRole.None)))
                .Where(i => i.ItemData.Count != 0)
                .OrderBy(i => i.Name)
                .ToListAsync();

            var itemStats = items.Select(i => new ItemStats(i)).ToList();

            return itemStats;
        }

        public async Task<ItemStats> GetItemStats(int id, ItemFilter filter)
        {
            var item = await _context.Items
                .Include(i => i.ItemData.Where(d =>
                    d.Rank == filter.Rank && d.Region == filter.Region &&
                    (d.Role == filter.Role || filter.Role == UggRole.None)))
                .Where(i => i.ItemData.Count != 0)
                .SingleAsync(i => i.Id == id);

            return new ItemStats(item);
        }
    }
}