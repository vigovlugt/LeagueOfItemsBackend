using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Application.Riot.Queries;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Riot;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Champions.Commands;

public record GetRiotChampionDataCommand(string Version) : IRequest<List<Champion>>;

public class GetRiotChampionDataCommandHandler : IRequestHandler<GetRiotChampionDataCommand, List<Champion>>
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetRiotChampionDataCommandHandler> _logger;

    public GetRiotChampionDataCommandHandler(IMediator mediator, IApplicationDbContext context,
        ILogger<GetRiotChampionDataCommandHandler> logger)
    {
        _mediator = mediator;
        _context = context;
        _logger = logger;
    }

    public async Task<List<Champion>> Handle(GetRiotChampionDataCommand request, CancellationToken cancellationToken)
    {
        var championResponse = await GetAllRiotChampionsResponse(request.Version);

        var riotChampionTasks =
            championResponse.Select((c) => GetRiotChampionResponse(request.Version, c.Id)).ToList();

        await Task.WhenAll(riotChampionTasks);

        var riotChampions = riotChampionTasks.Select(x => x.Result).ToList();

        var champions = ParseRiotChampions(riotChampions);

        _logger.LogInformation("{ChampionAmount} champions found", champions.Count);

        await SaveChampions(champions);

        return champions;
    }

    private async Task<List<RiotChampion>> GetAllRiotChampionsResponse(string version)
    {
        var responseStream = await _mediator.Send(new GetRiotApiResponse
        {
            Url = $"cdn/{version}/data/en_US/champion.json"
        });

        var championResponse = await JsonSerializer.DeserializeAsync<RiotChampionResponse>(responseStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (championResponse == null) throw new ArgumentException("LOL champions endpoint returned null");

        return championResponse.Data.Values.ToList();
    }

    private async Task<RiotChampion> GetRiotChampionResponse(string version, string riotId)
    {
        _logger.LogInformation("Downloading Riot champion data for {RiotId}", riotId);

        var responseStream = await _mediator.Send(new GetRiotApiResponse
        {
            Url = $"cdn/{version}/data/en_US/champion/{riotId}.json"
        });

        var championResponse = await JsonSerializer.DeserializeAsync<RiotChampionResponse>(responseStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (championResponse == null) throw new ArgumentException("LOL champions endpoint returned null");

        return championResponse.Data.Values.Single();
    }

    private async Task SaveChampions(List<Champion> champions)
    {
        var existing = _context.Champions.Where(c => champions.Select(c => c.Id).Contains(c.Id)).ToList();
        _context.Champions.RemoveRange(existing);
        _context.Champions.AddRange(champions);

        await _context.SaveChangesAsync();

        _logger.LogInformation("{ChampionAmount} champions saved", champions.Count);
    }

    private static List<Champion> ParseRiotChampions(List<RiotChampion> champions)
    {
        return champions.Select(c => new Champion(c)).ToList();
    }
}