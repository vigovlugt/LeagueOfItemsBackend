using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Ugg.Queries;

public record GetUggApiResponse : IRequest<Stream>
{
    public string Version { get; init; }
    public int ChampionId { get; init; }
    public string Type { get; init; }
    public bool Table { get; init; } = true;
}

public class GetUggApiResponseHandler : IRequestHandler<GetUggApiResponse, Stream>
{
    private readonly HttpClient _client;
    private readonly ILogger<GetUggApiResponseHandler> _logger;

    public GetUggApiResponseHandler(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<GetUggApiResponseHandler> logger)
    {
        _logger = logger;
        _client = clientFactory.CreateClient();
        _client.BaseAddress = new Uri(configuration["Ugg:ApiUrl"]);
        _client.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<Stream> Handle(GetUggApiResponse request, CancellationToken cancellationToken)
    {
        var uggVersion = string.Join('_', request.Version.Split(".").Take(2));

        var prefix = request.Table ? "table/" : "";
        var requestUri =
            $"lol/1.1/{prefix}{request.Type}/{uggVersion}/ranked_solo_5x5/{request.ChampionId}/1.5.0.json";

        HttpResponseMessage response = null;
        var success = false;
        var tries = 0;
        var shortCircuit = false;
        do
        {
            try
            {
                tries++;
                response = await _client.GetAsync(requestUri, cancellationToken);
                success = true;
            }
            catch (TaskCanceledException e) when (e.InnerException is TimeoutException)
            {
                _logger.LogWarning("Retrying UGG API Request {Url} Try {Try}", _client.BaseAddress + requestUri, tries);
            }
            catch (Exception e)
            {
                _logger.LogError("{Exception}", e);
                shortCircuit = true;
            }
            
        }
        while (!success && tries < 3 && !shortCircuit);

        if (response == null || !response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Could not resolve Ugg request: {Url}", _client.BaseAddress + requestUri);
            return null;
        }

        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}