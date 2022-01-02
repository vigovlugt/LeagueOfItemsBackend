using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Domain.Models.Champions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LeagueOfItems.Application.Champions.Queries;

public record GetChampionQuery : IRequest<ChampionStats>
{
    public int Id { get; init; }

    public GetChampionQuery(int id)
    {
        Id = id;
    }
}

public class GetChampionQueryHandler : IRequestHandler<GetChampionQuery, ChampionStats>
{
    private readonly IApplicationDbContext _context;

    public GetChampionQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ChampionStats> Handle(GetChampionQuery request, CancellationToken cancellationToken)
    {
        var champion = await _context.Champions
            .Include(c => c.ChampionData)
            .Include(c => c.ItemData)
            .Include(c => c.RuneData)
            .Include(c => c.BuildPathData)
            .SingleAsync(i => i.Id == request.Id, cancellationToken);

        return new ChampionStats(champion, champion.ItemData, champion.RuneData, champion.BuildPathData);
    }
}