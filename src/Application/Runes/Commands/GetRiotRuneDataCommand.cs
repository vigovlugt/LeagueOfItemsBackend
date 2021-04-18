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

namespace LeagueOfItems.Application.Runes.Commands
{
    public record GetRiotRuneDataCommand : IRequest
    {
        public GetRiotRuneDataCommand(string version)
        {
            Version = version;
        }
        
        public string Version { get; init; }
    }

    public class GetRiotRuneDataCommandHandler : IRequestHandler<GetRiotRuneDataCommand>
    {
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetRiotRuneDataCommandHandler> _logger;

        public GetRiotRuneDataCommandHandler(IMediator mediator, IApplicationDbContext context,
            ILogger<GetRiotRuneDataCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(GetRiotRuneDataCommand request, CancellationToken cancellationToken)
        {
            var runeResponse = await GetRuneResponse(request.Version);

            var runePaths = ParseRiotRuneResponse(runeResponse);

            await SaveRunePaths(runePaths);

            return Unit.Value;
        }

        private async Task<List<RiotRunePath>> GetRuneResponse(string version)
        {
            var responseStream = await _mediator.Send(new GetRiotApiResponse
            {
                Url = $"cdn/{version}/data/en_US/runesReforged.json"
            });

            var runeResponse = await JsonSerializer.DeserializeAsync<List<RiotRunePath>>(responseStream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return runeResponse;
        }

        private async Task SaveRunePaths(List<RunePath> runePaths)
        {
            await _mediator.Send(new DeleteAllRunesCommand());

            _context.RunePaths.AddRange(runePaths);

            await _context.SaveChangesAsync();

            _logger.LogInformation("{RuneAmount} runes saved", runePaths.SelectMany(x => x.Runes).Count());
        }

        private static List<RunePath> ParseRiotRuneResponse(List<RiotRunePath> runeResponse)
        {
            return runeResponse.Select(RunePath.FromRiotRunePath).ToList();
        }
    }
}