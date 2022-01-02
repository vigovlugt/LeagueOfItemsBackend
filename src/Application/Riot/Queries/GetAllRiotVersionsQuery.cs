using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Riot.Queries;

public record GetAllRiotVersionsQuery : IRequest<List<string>>;

public class GetAllRiotVersionsQueryHandler : IRequestHandler<GetAllRiotVersionsQuery, List<string>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetAllRiotVersionsQueryHandler> _logger;

    public GetAllRiotVersionsQueryHandler(IMediator mediator, ILogger<GetAllRiotVersionsQueryHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<List<string>> Handle(GetAllRiotVersionsQuery request, CancellationToken cancellationToken)
    {
        var responseStream = await _mediator.Send(new GetRiotApiResponse
        {
            Url = "api/versions.json"
        }, cancellationToken);

        var versions =
            await JsonSerializer.DeserializeAsync<List<string>>(responseStream,
                cancellationToken: cancellationToken);

        return versions;
    }
}