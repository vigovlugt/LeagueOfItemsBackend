using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LeagueOfItems.Application.Items.Queries
{
    public record GetItemQuery : IRequest<ItemStats>
    {
        public int Id { get; init; }
    }

    public class GetItemQueryHandler : IRequestHandler<GetItemQuery, ItemStats>
    {
        private readonly IApplicationDbContext _context;

        public GetItemQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemStats> Handle(GetItemQuery request, CancellationToken cancellationToken)
        {
            var item = await _context.Items
                .Include(i => i.ItemData)
                .Where(i => i.ItemData.Count != 0)
                .SingleAsync(i => i.Id == request.Id, cancellationToken);

            return new ItemStats(item);
        }
    }
}