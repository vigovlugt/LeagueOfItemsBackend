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
        public string Patch { get; init; }

        public GetAllChampionsQuery(string patch)
        {
            Patch = patch;
        }
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
                .Include(c => c.ChampionData.Where(d => d.Patch == request.Patch))
                .Include(c => c.ItemData.Where(d => d.Patch == request.Patch))
                .Include(c => c.RuneData.Where(d => d.Patch == request.Patch))
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var championStats = champions.Select(c => new ChampionStats(c, c.ItemData, c.RuneData)).ToList();

            return championStats;
        }
    }
}