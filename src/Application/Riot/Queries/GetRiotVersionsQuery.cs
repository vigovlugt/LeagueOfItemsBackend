using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Riot.Queries;

public record GetRiotVersionsQuery : IRequest<List<string>>;

public class GetRiotVersionsQueryHandler : IRequestHandler<GetRiotVersionsQuery, List<string>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetRiotVersionsQueryHandler> _logger;

    public GetRiotVersionsQueryHandler(IMediator mediator, ILogger<GetRiotVersionsQueryHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<List<string>> Handle(GetRiotVersionsQuery request, CancellationToken cancellationToken)
    {
        var versions = await GetRiotVersionResponse(cancellationToken);

        return versions;
    }

    private async Task<List<string>> GetRiotVersionResponse(CancellationToken cancellationToken)
    {
        var responseStream = await _mediator.Send(new GetRiotApiResponse
        {
            Url = "api/versions.json"
        }, cancellationToken);

        var versions =
            await JsonSerializer.DeserializeAsync<List<string>>(responseStream,
                cancellationToken: cancellationToken);

        if (versions == null || versions.Count == 0) throw new Exception("No LOL version found");

        return versions;
    }
}