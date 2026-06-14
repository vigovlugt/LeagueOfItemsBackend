using System;
using System.IO;
using System.Linq;
using System.Net;
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
    private const int MaxTries = 5;
    private const int DefaultRequestDelayMs = 1250;
    private const int DefaultForbiddenRetryDelaySeconds = 60;
    private const string UggOrigin = "https://u.gg";
    private const string BrowserUserAgent =
        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36";

    private readonly HttpClient _client;
    private readonly ILogger<GetUggApiResponseHandler> _logger;
    private readonly TimeSpan _requestDelay;
    private readonly TimeSpan _forbiddenRetryDelay;

    public GetUggApiResponseHandler(IHttpClientFactory clientFactory, IConfiguration configuration,
        ILogger<GetUggApiResponseHandler> logger)
    {
        _logger = logger;
        _client = clientFactory.CreateClient();
        _client.BaseAddress = new Uri(configuration["Ugg:ApiUrl"]);
        _client.Timeout = TimeSpan.FromSeconds(60);
        _client.DefaultRequestHeaders.UserAgent.ParseAdd(BrowserUserAgent);
        _client.DefaultRequestHeaders.Accept.ParseAdd("application/json, text/plain, */*");
        _client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
        _client.DefaultRequestHeaders.Add("Origin", UggOrigin);
        _client.DefaultRequestHeaders.Referrer = new Uri($"{UggOrigin}/");

        _requestDelay = TimeSpan.FromMilliseconds(GetConfiguredInt(configuration["Ugg:RequestDelayMs"],
            DefaultRequestDelayMs));
        _forbiddenRetryDelay = TimeSpan.FromSeconds(GetConfiguredInt(configuration["Ugg:ForbiddenRetryDelaySeconds"],
            DefaultForbiddenRetryDelaySeconds));
    }

    public async Task<Stream> Handle(GetUggApiResponse request, CancellationToken cancellationToken)
    {
        var uggVersion = string.Join('_', request.Version.Split(".").Take(2));

        var prefix = request.Table ? "table/" : "";
        var requestUri =
            $"lol/1.5/{prefix}{request.Type}/{uggVersion}/ranked_solo_5x5/{request.ChampionId}/1.5.0.json";

        HttpResponseMessage response = null;
        for (var tries = 1; tries <= MaxTries; tries++)
        {
            try
            {
                await Task.Delay(_requestDelay, cancellationToken);
                response = await _client.GetAsync(requestUri, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStreamAsync(cancellationToken);
                }

                if (!ShouldRetry(response.StatusCode) || tries == MaxTries)
                {
                    break;
                }

                if (ShouldRetry(response.StatusCode))
                {
                    _logger.LogWarning("Retrying UGG API Request {Url} Try {Try} Status: {StatusCode}",
                        _client.BaseAddress + requestUri, tries, response.StatusCode);

                    await Task.Delay(GetRetryDelay(response, tries), cancellationToken);
                }
            }
            catch (Exception e) when (IsTransient(e) && tries < MaxTries)
            {
                _logger.LogWarning(e, "Retrying UGG API Request {Url} Try {Try}", _client.BaseAddress + requestUri,
                    tries);

                await Task.Delay(TimeSpan.FromSeconds(tries * 2), cancellationToken);
            }
        }

        if (response == null || !response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Could not resolve Ugg request: {Url} Status: {StatusCode}",
                _client.BaseAddress + requestUri, response?.StatusCode);
            return null;
        }

        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    private static bool ShouldRetry(HttpStatusCode statusCode)
    {
        return statusCode == HttpStatusCode.Forbidden || statusCode == HttpStatusCode.TooManyRequests ||
               (int)statusCode >= 500;
    }

    private static bool IsTransient(Exception exception)
    {
        return exception is HttpRequestException ||
               exception is TaskCanceledException { InnerException: TimeoutException };
    }

    private TimeSpan GetRetryDelay(HttpResponseMessage response, int tries)
    {
        return response.Headers.RetryAfter?.Delta ??
               (response.StatusCode switch
               {
                   HttpStatusCode.Forbidden => _forbiddenRetryDelay,
                   HttpStatusCode.TooManyRequests => TimeSpan.FromSeconds(30),
                   _ => TimeSpan.FromSeconds(tries * 2)
               });
    }

    private static int GetConfiguredInt(string configuredValue, int defaultValue)
    {
        return int.TryParse(configuredValue, out var value) && value > 0 ? value : defaultValue;
    }
}
