using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Domain.Models.Items;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LeagueOfItems.Application.Items.Queries;

public record GetAllItemsQuery : IRequest<List<ItemStats>>
{
    public string Patch { get; init; }

    public GetAllItemsQuery(string patch)
    {
        Patch = patch;
    }
}

public class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, List<ItemStats>>
{
    private readonly IApplicationDbContext _context;

    public GetAllItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ItemStats>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.Items
            .Include(i => i.ItemData.Where(d => d.Patch == request.Patch))
            .ThenInclude(i => i.Champion).ThenInclude(c => c.ChampionData.Where(d => d.Patch == request.Patch))
            .Where(i => i.ItemData.Count != 0)
            .OrderBy(i => i.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var itemStats = items.Select(i => new ItemStats(i)).ToList();

        return itemStats;
    }
}