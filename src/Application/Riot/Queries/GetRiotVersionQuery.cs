using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Riot.Queries
{
    public record GetRiotVersionQuery : IRequest<string>
    {
    }

    public class GetRiotVersionQueryHandler : IRequestHandler<GetRiotVersionQuery, string>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetRiotVersionQueryHandler> _logger;

        public GetRiotVersionQueryHandler(IMediator mediator, ILogger<GetRiotVersionQueryHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<string> Handle(GetRiotVersionQuery request, CancellationToken cancellationToken)
        {
            var version = await GetRiotVersionResponse(cancellationToken);

            return version;
        }

        private async Task<string> GetRiotVersionResponse(CancellationToken cancellationToken)
        {
            var responseStream = await _mediator.Send(new GetRiotApiResponse
            {
                Url = "api/versions.json"
            }, cancellationToken);

            var versions =
                await JsonSerializer.DeserializeAsync<List<string>>(responseStream,
                    cancellationToken: cancellationToken);

            if (versions == null || versions.Count == 0) throw new Exception("No LOL version found");

            var version = versions[0];

            _logger.LogInformation("Current Riot LoL version {Version}", version);

            return version;
        }
    }
}