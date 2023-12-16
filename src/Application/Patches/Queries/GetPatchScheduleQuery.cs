using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using LeagueOfItems.Domain.Models.Patches;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Patches.Queries;

public record GetPatchScheduleQuery : IRequest<List<ScheduledPatch>>;

record ZendeskApiResponse
{
    public ZendeskApiArticle Article { get; set; }
}

record ZendeskApiArticle
{
    public string Body { get; set; }
}

public class GetPatchScheduleQueryHandler : IRequestHandler<GetPatchScheduleQuery, List<ScheduledPatch>>
{
    private readonly HttpClient _client;
    private readonly Uri _patchScheduleUri;
    private readonly ILogger<GetPatchScheduleQueryHandler> _logger;

    public GetPatchScheduleQueryHandler(ILogger<GetPatchScheduleQueryHandler> logger, IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _client = clientFactory.CreateClient();
        _logger = logger;
        _patchScheduleUri = new Uri(configuration["Riot:PatchScheduleUrl"]);
    }

    public async Task<List<ScheduledPatch>> Handle(GetPatchScheduleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting Patch Schedule from {PatchScheduleUrl}", _patchScheduleUri.ToString());
        var response = await _client.GetAsync(_patchScheduleUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await JsonSerializer.DeserializeAsync<ZendeskApiResponse>(await response.Content.ReadAsStreamAsync(cancellationToken), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }, cancellationToken);

        var doc = new HtmlDocument();
        doc.LoadHtml(json.Article.Body);

        var tableRows = doc.DocumentNode.SelectNodes("//table/tbody/tr");

        return tableRows.Select(r =>
        {
            var tds = r.SelectNodes("td");
            return new ScheduledPatch(tds[0].InnerHtml, ParseDateString(tds[1].SelectSingleNode("span").InnerHtml));
        }).ToList();
    }

    private static DateTime ParseDateString(string dateString)
    {
        dateString = dateString.Replace("Sept", "Sep");
        dateString = dateString.Replace("Wednesday ", "Wednesday, ");

        return DateTime.Parse(string.Join(" ", dateString.Split(",").Select(x => x.Trim()).Skip(1)));
    }
}
