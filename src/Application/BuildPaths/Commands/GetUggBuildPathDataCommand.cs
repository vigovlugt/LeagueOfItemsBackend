using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.BuildPaths.Services;
using LeagueOfItems.Application.Champions.Services;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Application.Ugg.Queries;
using LeagueOfItems.Application.Ugg.Services;
using LeagueOfItems.Domain.Models.BuildPaths;
using LeagueOfItems.Domain.Models.Champions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.BuildPaths.Commands;

public record GetUggBuildPathDataCommand(string Version) : IRequest<List<BuildPathData>>;

public class GetUggBuildPathDataCommandHandler : IRequestHandler<GetUggBuildPathDataCommand, List<BuildPathData>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetUggBuildPathDataCommandHandler> _logger;
    private readonly IMediator _mediator;

    public GetUggBuildPathDataCommandHandler(ILogger<GetUggBuildPathDataCommandHandler> logger,
        IApplicationDbContext context,
        IMediator mediator)
    {
        _logger = logger;
        _context = context;
        _mediator = mediator;
    }

    public async Task<List<BuildPathData>> Handle(GetUggBuildPathDataCommand request, CancellationToken cancellationToken)
    {
        var champions = await _context.Champions.ToListAsync(cancellationToken);
            
        var buildPathDataTasks =
            champions.Select(champion => GetBuildPathDataForChampion(request.Version, champion)).ToList();
            
        await Task.WhenAll(buildPathDataTasks);

        var buildPathData = buildPathDataTasks.SelectMany(x => x.Result).ToList();

        await SaveBuildPathData(buildPathData);

        return buildPathData;
    }

    private async Task<List<BuildPathData>> GetBuildPathDataForChampion(string version, Champion champion)
    {
        _logger.LogInformation("Downloading BuildPath data for {Champion} Version: {Version}", champion.Name,
            version);

        await using var responseStream = await _mediator.Send(new GetUggApiResponse
        {
            ChampionId = champion.Id,
            Type = "builds",
            Version = version,
            Table = false
        });

        if (responseStream == null)
        {
            _logger.LogWarning("No Ugg response for BuildPathData for Champion {Champion}", champion.Name);
            return new List<BuildPathData>();
        }

        var parsedBuildPathData = await UggBuildPathDataParser.Parse(champion.Id, responseStream, version);

        var filteredBuildPathData = UggDataFilterer.Filter(parsedBuildPathData);

        return filteredBuildPathData;
    }
        
    private async Task SaveBuildPathData(List<BuildPathData> buildPathData)
    {
        _context.BuildPathData.AddRange(buildPathData);

        await _context.SaveChangesAsync();

        _logger.LogInformation("{BuildPathDataAmount} BuildPathData rows saved", buildPathData.Count);
    }
}