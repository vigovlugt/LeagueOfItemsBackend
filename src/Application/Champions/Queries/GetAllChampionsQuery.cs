using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Domain.Models.Champions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LeagueOfItems.Application.Champions.Queries
{
    public record GetAllChampionsQuery : IRequest<List<ChampionStats>>
    {
    }

    public class GetAllChampionsQueryHandler : IRequestHandler<GetAllChampionsQuery, List<ChampionStats>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllChampionsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChampionStats>> Handle(GetAllChampionsQuery request, CancellationToken cancellationToken)
        {
            var champions = await _context.Champions
                .Include(c => c.ChampionData)
                .Include(c => c.ItemData)
                .Include(c => c.RuneData)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);

            var championStats = champions.Select(c => new ChampionStats(c, c.ItemData, c.RuneData)).ToList();

            return championStats;
        }
    }
}