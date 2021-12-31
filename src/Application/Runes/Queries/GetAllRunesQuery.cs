using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Domain.Models.Runes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LeagueOfItems.Application.Runes.Queries
{
    public record GetAllRunesQuery(string Patch) : IRequest<List<RuneStats>>;

    public class GetAllRunesQueryHandler : IRequestHandler<GetAllRunesQuery, List<RuneStats>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllRunesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RuneStats>> Handle(GetAllRunesQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.Runes
                .Include(r => r.RunePath)
                .Include(i => i.RuneData.Where(d => d.Patch == request.Patch))
                .ThenInclude(i => i.Champion).ThenInclude(c => c.ChampionData.Where(d => d.Patch == request.Patch))
                .Where(i => i.RuneData.Count != 0)
                .OrderBy(i => i.Name)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var runeStats = items.Select(r => new RuneStats(r)).ToList();

            return runeStats;
        }
    }
}