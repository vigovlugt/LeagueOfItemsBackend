using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Lolalytics.Models;
using LeagueOfItems.Application.Lolalytics.Services;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace LeagueOfItems.Application.Lolalytics.Queries;

public record GetLolalyticsQwikChampionQuery(string Version, string ChampionId, string Role = null)
    : IRequest<LolalyticsQwikChampionData>;

public class GetLolalyticsQwikChampionQueryHandler
    : IRequestHandler<GetLolalyticsQwikChampionQuery, LolalyticsQwikChampionData>
{
    private readonly HttpClient _client;

    public GetLolalyticsQwikChampionQueryHandler(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _client = clientFactory.CreateClient();
        _client.BaseAddress = new Uri(configuration["Lolalytics:BaseUrl"]);
        _client.Timeout = TimeSpan.FromSeconds(60);
    }

    public async Task<LolalyticsQwikChampionData> Handle(GetLolalyticsQwikChampionQuery request,
        CancellationToken cancellationToken)
    {
        var championSlug = LolalyticsRequestHelper.GetChampionSlug(request.ChampionId);
        var patch = LolalyticsRequestHelper.GetPatch(request.Version);
        var queryParams = new Dictionary<string, string>
        {
            ["tier"] = "emerald_plus",
            ["region"] = "all",
            ["patch"] = patch,
            ["lane"] = request.Role
        };
        var requestUri = $"lol/{championSlug}/build/?{LolalyticsRequestHelper.ToQueryString(queryParams)}";

        using var response = await _client.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync(cancellationToken);
        return LolalyticsQwikParser.Parse<LolalyticsQwikChampionData>(html);
    }
}
