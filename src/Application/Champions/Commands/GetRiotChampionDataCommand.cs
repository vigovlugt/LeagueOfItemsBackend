using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Application.Riot.Queries;
using LeagueOfItems.Domain.Models;
using LeagueOfItems.Domain.Models.Riot;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Champions.Commands
{
    public record GetRiotChampionDataCommand : IRequest
    {
        public GetRiotChampionDataCommand(string version)
        {
            Version = version;
        }
        
        public string Version { get; init; }
    }

    public class GetRiotChampionDataCommandHandler : IRequestHandler<GetRiotChampionDataCommand>
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

        public async Task<Unit> Handle(GetRiotChampionDataCommand request, CancellationToken cancellationToken)
        {
            var championResponse = await GetRiotChampionResponse(request.Version);

            var champions = ParseRiotChampions(championResponse);

            _logger.LogInformation("{ChampionAmount} champions found", champions.Count);

            await SaveChampions(champions);

            return Unit.Value;
        }

        private async Task<List<RiotChampion>> GetRiotChampionResponse(string version)
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

        private async Task SaveChampions(List<Champion> champions)
        {
            await _mediator.Send(new DeleteAllChampionsCommand());

            _context.Champions.AddRange(champions);

            await _context.SaveChangesAsync();

            _logger.LogInformation("{ChampionAmount} champions saved", champions.Count);
        }

        private static List<Champion> ParseRiotChampions(List<RiotChampion> champions)
        {
            return champions.Select(Champion.FromRiotChampion).ToList();
        }
    }
}