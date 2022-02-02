using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using LeagueOfItems.Domain.Models.Patches;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Patches.Queries;

public record GetPatchNotesQuery(string Patch) : IRequest<PatchNotesDataset>;

public class GetPatchNotesQueryHandler : IRequestHandler<GetPatchNotesQuery, PatchNotesDataset>
{
    private readonly HttpClient _client;
    private readonly string _patchNotesUrl;
    private readonly ILogger<GetPatchNotesQueryHandler> _logger;

    public GetPatchNotesQueryHandler(ILogger<GetPatchNotesQueryHandler> logger, IHttpClientFactory clientFactory,
        IConfiguration configuration)
    {
        _client = clientFactory.CreateClient();
        _logger = logger;
        _patchNotesUrl = configuration["Riot:PatchNotesUrl"];
    }

    public async Task<PatchNotesDataset> Handle(GetPatchNotesQuery request, CancellationToken cancellationToken)
    {
        var url = _patchNotesUrl.Replace("{}", request.Patch.Replace(".", "-"));

        _logger.LogInformation("Getting Patch Notes from {PatchNotesUrl}", url);
        var response = await _client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await JsonSerializer.DeserializeAsync<JsonNode>(
            await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);

        return ParseJson(json);
    }

    private PatchNotesDataset ParseJson(JsonNode json)
    {
        var data = json["result"]["data"]["all"]["nodes"][0];
        var title = data["title"].ToString();
        var description = data["description"].ToString();
        var bannerUrl = data["banner"]["url"].ToString();
        var html = data["patch_notes_body"][0]["patch_notes"]["html"].ToString();
        
        _logger.LogInformation("Got PatchNotes html {Html}", html);

        var document = new HtmlDocument();
        document.LoadHtml(html);

        var container = document.DocumentNode.ChildNodes[0];

        var quote = "";
        string highlightUrl = null;

        PatchNotesGroup currentGroup = null;
        var groups = new List<PatchNotesGroup>();

        foreach (var node in container.ChildNodes)
        {
            if (node.HasClass("blockquote") && node.HasClass("context"))
            {
                quote = new Regex("[\t\n]+").Replace(node.InnerText.Trim(new char[] {' ', '\n', '\t'}), "\n\n");
            }
            else if (node.HasClass("header-primary"))
            {
                var h2 = node.QuerySelector("h2");
                currentGroup = new PatchNotesGroup(h2.Id, h2.InnerText);
                groups.Add(currentGroup);
            }
            else if (node.HasClass("content-border"))
            {
                var patchChangeBlock = node.QuerySelector("div.white-stone.accent-before");
                if (currentGroup != null && currentGroup.Id == "patch-patch-highlights")
                {
                    highlightUrl = node.QuerySelector("img").GetAttributeValue("src", null);
                }
                else if (patchChangeBlock != null)
                {
                    var change = ParseChange(patchChangeBlock);
                    if (change != null)
                    {
                        currentGroup.Changes.Add(change);
                    }
                }
            }
        }

        return new PatchNotesDataset
        {
            Title = title,
            Description = description,
            BannerImageUrl = bannerUrl,
            HighlightImageUrl = highlightUrl,
            Quote = quote,
            Groups = groups.Where(g => g.Changes.Any()).ToList()
        };
    }

    private PatchNotesChange ParseChange(HtmlNode node)
    {
        node = node.QuerySelector("div");
        if (node.QuerySelector(".change-title") == null)
        {
            return null;
        }
        
        var change = new PatchNotesChange
        {
            Title = (node.QuerySelector(".change-title a") ?? node.QuerySelector(".change-title")).ChildNodes.FindFirst("#text").InnerText.Trim(),
            Summary = node.QuerySelector(".summary")?.InnerText.Trim(),
            Quote = node.QuerySelector(".blockquote.context")?.InnerText.Replace("\t", "").Trim(new char[] {' ', '\n'}),
        };

        foreach (var htmlNode in node.ChildNodes)
        {
            if (htmlNode.HasClass("attribute-change"))
            {
                change.AddAttributeChange(ParseAttributeChange(htmlNode));
            }
            else if (htmlNode.HasClass("change-detail-title"))
            {
                change.Details.Add(new PatchNotesChangeDetail
                {
                    Title = htmlNode.InnerText.Trim()
                });
            }
        }

        return change;
    }

    private PatchNotesAttributeChange ParseAttributeChange(HtmlNode node)
    {
        var attributeNode = node.QuerySelector(".attribute");

        return new PatchNotesAttributeChange
        {
            Attribute = attributeNode.ChildNodes.Last().InnerText.Trim(),
            ChangeType = attributeNode.ChildNodes.Count > 1 ? attributeNode.ChildNodes[0].InnerText.Trim() : null,
            Before = node.QuerySelector(".attribute-before")?.InnerText.Trim(),
            After = node.QuerySelector(".attribute-after")?.InnerText.Trim(),
            Removed = node.QuerySelector(".attribute-removed")?.InnerText.Trim()
        };
    }
}