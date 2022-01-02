using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace LeagueOfItems.Application.Riot.Queries;

public record GetRiotApiResponse : IRequest<Stream>
{
    public string Url { get; init; }
}

public class GetRiotApiResponseHandler : IRequestHandler<GetRiotApiResponse, Stream>
{
    private readonly HttpClient _client;

    public GetRiotApiResponseHandler(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _client = clientFactory.CreateClient();
        _client.BaseAddress = new Uri(configuration["Riot:ApiUrl"]);
    }

    public async Task<Stream> Handle(GetRiotApiResponse request, CancellationToken cancellationToken)
    {
        var response = await _client.GetAsync(request.Url, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}