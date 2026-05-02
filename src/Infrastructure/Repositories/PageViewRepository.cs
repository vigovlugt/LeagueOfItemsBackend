using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Domain.Models.PageViews;
using Microsoft.Extensions.Configuration;

namespace LeagueOfItems.Infrastructure.Repositories;

public class PageViewRepository : IPageViewRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _client;
    private readonly string _accountId;
    private readonly string _apiToken;
    private readonly string _dataset;
    private readonly int _lookbackDays;

    public PageViewRepository(
        IHttpClientFactory clientFactory,
        IConfiguration config)
    {
        _client = clientFactory.CreateClient();
        _accountId = config["Cloudflare:AccountId"];
        _apiToken = config["Cloudflare:AnalyticsApiToken"];
        _dataset = config["Cloudflare:PageViewsDataset"] ?? "pageviews";
        _lookbackDays = int.TryParse(config["Cloudflare:PageViewsLookbackDays"], out var lookbackDays)
            ? lookbackDays
            : 7;
    }

    public async Task<PageViewDataset> GetDatasetAsync()
    {
        if (string.IsNullOrWhiteSpace(_accountId))
        {
            throw new InvalidOperationException("Cloudflare:AccountId configuration is required");
        }

        if (string.IsNullOrWhiteSpace(_apiToken))
        {
            throw new InvalidOperationException("Cloudflare:AnalyticsApiToken configuration is required");
        }

        if (!Regex.IsMatch(_dataset, "^[A-Za-z_][A-Za-z0-9_]*$"))
        {
            throw new InvalidOperationException("Cloudflare:PageViewsDataset must be a valid Analytics Engine table name");
        }

        if (_lookbackDays <= 0)
        {
            throw new InvalidOperationException("Cloudflare:PageViewsLookbackDays must be greater than zero");
        }

        var sql = $"""
                  SELECT
                      blob1 AS type,
                      blob2 AS id,
                      SUM(_sample_interval * double1) AS count
                  FROM {_dataset}
                  WHERE timestamp >= NOW() - INTERVAL '{_lookbackDays}' DAY
                    AND blob1 IN ('ITEM', 'RUNE', 'CHAMPION')
                  GROUP BY type, id
                  ORDER BY count DESC
                  FORMAT JSON
                  """;

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://api.cloudflare.com/client/v4/accounts/{_accountId}/analytics_engine/sql")
        {
            Content = new StringContent(sql, Encoding.UTF8, "text/plain")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);

        using var response = await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Cloudflare Analytics Engine SQL API returned {(int)response.StatusCode}: {content}");
        }

        var result = JsonSerializer.Deserialize<AnalyticsEngineResponse>(content, JsonOptions)
                     ?? new AnalyticsEngineResponse();

        var pageViewResults = result.Data.Select(v => new PageViewData
        {
            Id = int.Parse(v.Id),
            Type = v.Type,
            Count = (int)Math.Round(v.Count)
        }).OrderByDescending(p => p.Count).ToList();

        return new PageViewDataset(pageViewResults);
    }

    private class AnalyticsEngineResponse
    {
        [JsonPropertyName("data")]
        public List<AnalyticsEnginePageViewRow> Data { get; set; } = new();
    }

    private class AnalyticsEnginePageViewRow
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("count")]
        public double Count { get; set; }
    }
}