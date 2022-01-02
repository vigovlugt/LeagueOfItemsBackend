using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using LeagueOfItems.Domain.Models.Patches;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Patches.Queries;

public record GetPatchScheduleQuery : IRequest<List<ScheduledPatch>>;

public class GetPatchScheduleQueryHandler : IRequestHandler<GetPatchScheduleQuery, List<ScheduledPatch>>
{
    private readonly HttpClient _client;
    private readonly Uri _patchScheduleUri;
    private readonly ILogger<GetPatchScheduleQueryHandler> _logger;

    public GetPatchScheduleQueryHandler(ILogger<GetPatchScheduleQueryHandler> logger, IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _client = clientFactory.CreateClient();
        _client.DefaultRequestHeaders.UserAgent.ParseAdd("LeagueOfItems");
        _logger = logger;
        _patchScheduleUri = new Uri(configuration["Riot:PatchScheduleUrl"]);
    }
    
    public async Task<List<ScheduledPatch>> Handle(GetPatchScheduleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting Patch Schedule from {PatchScheduleUrl}", _patchScheduleUri.ToString());
        var response = await _client.GetAsync(_patchScheduleUri, cancellationToken);

        response.EnsureSuccessStatusCode();
        
        var doc = new HtmlDocument();
        doc.LoadHtml(await response.Content.ReadAsStringAsync(cancellationToken));

        var tableRows = doc.DocumentNode.SelectNodes("//*[@id=\"top\"]/table/tbody/tr");

        return tableRows.Select(r =>
        {
            var tds = r.SelectNodes("td");
            return new ScheduledPatch(tds[0].InnerHtml, ParseDateString(tds[1].InnerHtml));
        }).ToList();
    }

    private static DateTime ParseDateString(string dateString)
    {
        return DateTime.Parse(string.Join(" ", dateString.Split(",").Select(x => x.Trim()).Skip(1)));
    }
}