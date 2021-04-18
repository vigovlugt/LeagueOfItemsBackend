using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LeagueOfItems.Application.Runes.Queries
{
    public record GetRuneQuery : IRequest<RuneStats>
    {
        public int Id { get; init; }
    }

    public class GetRuneQueryHandler : IRequestHandler<GetRuneQuery, RuneStats>
    {
        private readonly IApplicationDbContext _context;

        public GetRuneQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RuneStats> Handle(GetRuneQuery request, CancellationToken cancellationToken)
        {
            var rune = await _context.Runes
                .Include(r => r.RunePath)
                .Where(i => i.RuneData.Count != 0)
                .SingleAsync(i => i.Id == request.Id, cancellationToken);

            return new RuneStats(rune);
        }
    }
}