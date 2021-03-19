using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LeagueOfItems.Models;
using LeagueOfItems.Models.Ugg;
using LeagueOfItems.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Services
{
    public interface IItemService
    {
        Task<List<ItemViewModel>> GetAllItems(ItemFilter filter);
        Task<ItemViewModel> GetItemStats(int id, ItemFilter filter);
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

        public async Task<List<ItemViewModel>> GetAllItems(ItemFilter filter)
        {
            var items = await _context.Items
                .Include(i => i.ItemData.Where(d =>
                    d.Rank == filter.Rank && d.Region == filter.Region &&
                    (d.Role == filter.Role || filter.Role == UggRole.None)))
                .Where(i => i.ItemData.Count != 0)
                .OrderBy(i => i.Name)
                .ToListAsync();

            var itemVms = _mapper.Map<List<ItemViewModel>>(items);
            foreach (var itemVm in itemVms)
            {
                itemVm.Wins = itemVm.ItemData.Select(d => d.Wins).Aggregate(0, (a, b) => a + b);
                itemVm.Matches = itemVm.ItemData.Select(d => d.Matches).Aggregate(0, (a, b) => a + b);
                itemVm.ItemData = null;
            }

            return itemVms;
        }
        
        public async Task<ItemViewModel> GetItemStats(int id, ItemFilter filter)
        {
            var item = await _context.Items
                .Include(i => i.ItemData.Where(d =>
                    d.Rank == filter.Rank && d.Region == filter.Region &&
                    (d.Role == filter.Role || filter.Role == UggRole.None)))
                .Where(i => i.ItemData.Count != 0)
                .SingleAsync(i => i.Id == id);
            
            var itemVm = _mapper.Map<ItemViewModel>(item);
            
            itemVm.ItemData.ForEach(d =>
            {
                d.Item = null;
            });

            return itemVm;
        }
    }
}