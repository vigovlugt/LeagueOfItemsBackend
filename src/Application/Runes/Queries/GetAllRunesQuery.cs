using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LeagueOfItems.Application.Runes.Queries
{
    public record GetAllRunesQuery : IRequest<List<RuneStats>>
    {
    }

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
                .Include(i => i.RuneData)
                .Where(i => i.RuneData.Count != 0)
                .OrderBy(i => i.Name)
                .ToListAsync(cancellationToken);

            var runeStats = items.Select(r => new RuneStats(r)).ToList();

            return runeStats;
        }
    }
}