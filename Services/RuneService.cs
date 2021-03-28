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
    public interface IRuneService
    {
        Task<List<RuneStats>> GetAllRunes(ItemFilter filter);
        Task<RuneStats> GetRuneStats(int id, ItemFilter filter);
    }

    public class RuneService : IRuneService
    {
        private readonly ILogger<RuneService> _logger;
        private readonly ItemContext _context;
        private readonly IMapper _mapper;

        public RuneService(ILogger<RuneService> logger, ItemContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RuneStats>> GetAllRunes(ItemFilter filter)
        {
            var items = await _context.Runes
                .Include(r => r.RunePath)
                .Include(i => i.RuneData.Where(d =>
                    d.Rank == filter.Rank && d.Region == filter.Region &&
                    (d.Role == filter.Role || filter.Role == UggRole.None)))
                .Where(i => i.RuneData.Count != 0)
                .OrderBy(i => i.Name)
                .ToListAsync();

            var runeStats = items.Select(r => new RuneStats(r)).ToList();

            return runeStats;
        }

        public async Task<RuneStats> GetRuneStats(int id, ItemFilter filter)
        {
            var rune = await _context.Runes
                .Include(r => r.RunePath)
                .Include(i => i.RuneData.Where(d =>
                    d.Rank == filter.Rank && d.Region == filter.Region &&
                    (d.Role == filter.Role || filter.Role == UggRole.None)))
                .Where(i => i.RuneData.Count != 0)
                .SingleAsync(i => i.Id == id);

            return new RuneStats(rune);
        }
    }
}