using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LeagueOfItems.Models;
using LeagueOfItems.Models.Filters;
using LeagueOfItems.Models.Ugg;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Services
{
    public interface IItemService
    {
        Task<List<ItemStats>> GetAllItems(ItemFilter filter);
        Task<ItemStats> GetItemStats(int id, ItemFilter filter);
    }

    public class ItemService : IItemService
    {
        private readonly ILogger<ItemService> _logger;
        private readonly ItemContext _context;
        private readonly IMapper _mapper;

        public ItemService(ILogger<ItemService> logger, ItemContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
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