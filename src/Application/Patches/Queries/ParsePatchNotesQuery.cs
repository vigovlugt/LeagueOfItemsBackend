using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using LeagueOfItems.Domain.Models.Patches;
using LeagueOfItems.Application.Ugg.Helpers;
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
        // var major = int.Parse(request.Patch.Split('.')[0]) + 2010;
        // var minor = int.Parse(request.Patch.Split('.')[1]);
        // var split = (minor - 1) / 8 + 1;
        // var patch = (minor - 1) % 8 + 1;
        // var seasonOnePatch = $"{major}-s{split}-{patch}";
        // var url = _patchNotesUrl.Replace("{}", seasonOnePatch);
        // var url = _patchNotesUrl.Replace("{}", request.Patch.Replace(".", "-"));
        var url = _patchNotesUrl.Replace("{}", LolVersionHelper.GetPublicPatchSlug(request.Patch));

        _logger.LogInformation("Getting Patch Notes from {PatchNotesUrl}", url);
        var response = await _client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync(cancellationToken);

        var document = new HtmlDocument();
        document.LoadHtml(html);

        var nextData = document.DocumentNode.QuerySelector("script#__NEXT_DATA__");

        var json = JsonSerializer.Deserialize<JsonNode>(nextData.InnerText);

        return ParseJson(json);
    }

    private PatchNotesDataset ParseJson(JsonNode json)
    {
        var data = json["props"]["pageProps"]["page"]["blades"];
        var patchNotesRichText = data.AsArray().First(b => b["type"].ToString() == "patchNotesRichText")["richText"]["body"].ToString();
        var masthead = data.AsArray().First(b => b["type"].ToString() == "articleMasthead");
        var title = masthead["title"].ToString();
        var description = masthead["description"]["body"].ToString();
        var bannerUrl = masthead["banner"]["url"].ToString();

        var document = new HtmlDocument();
        document.LoadHtml(patchNotesRichText);

        var container = document.DocumentNode.ChildNodes[0];

        var quote = "";
        string highlightUrl = null;

        PatchNotesGroup currentGroup = null;
        var groups = new List<PatchNotesGroup>();

        foreach (var node in container.ChildNodes)
        {
            if (node.HasClass("blockquote") && node.HasClass("context"))
            {
                quote = new Regex("[\t\n]+").Replace(node.InnerText.Trim(new char[] { ' ', '\n', '\t' }), "\n\n");
            }
            else if (node.HasClass("header-primary"))
            {
                var h2 = node.QuerySelector("h2");
                currentGroup = new PatchNotesGroup(h2.Id, NormalizeText(h2.InnerText));
                groups.Add(currentGroup);
            }
            else if (node.HasClass("content-border"))
            {
                var patchChangeBlock = node.QuerySelector("div.white-stone.accent-before");
                if (currentGroup != null && currentGroup.Id == "patch-patch-highlights")
                {
                    highlightUrl = node.QuerySelector("img")?.GetAttributeValue("src", null) ?? highlightUrl;
                }
                else if (patchChangeBlock != null)
                {
                    currentGroup.Changes.AddRange(ParseChanges(patchChangeBlock, currentGroup.Title));
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

    private List<PatchNotesChange> ParseChanges(HtmlNode node, string fallbackTitle)
    {
        var content = node.QuerySelector("div");
        if (content == null)
        {
            return new List<PatchNotesChange>();
        }

        var titleNode = content.QuerySelector(".change-title");
        if (titleNode != null)
        {
            return new List<PatchNotesChange>
            {
                new()
                {
                    Title = GetChangeTitle(titleNode),
                    Body = ToMarkdown(GetNodesAfter(content.ChildNodes, titleNode))
                }
            };
        }

        var changes = ParseChangesByDetailTitle(content);
        if (changes.Any())
        {
            return changes;
        }

        var body = ToMarkdown(content.ChildNodes);
        return string.IsNullOrWhiteSpace(body)
            ? new List<PatchNotesChange>()
            : new List<PatchNotesChange>
            {
                new()
                {
                    Title = GetFallbackTitle(content, fallbackTitle),
                    Body = body
                }
            };
    }

    private List<PatchNotesChange> ParseChangesByDetailTitle(HtmlNode content)
    {
        var changes = new List<PatchNotesChange>();
        PatchNotesChange currentChange = null;
        var bodyNodes = new List<HtmlNode>();
        var pendingHeadings = new List<string>();

        foreach (var node in content.ChildNodes)
        {
            if (node.HasClass("change-detail-title"))
            {
                AddChange(changes, currentChange, bodyNodes, pendingHeadings);
                currentChange = new PatchNotesChange
                {
                    Title = NormalizeText(node.InnerText)
                };
                bodyNodes = new List<HtmlNode>();
                continue;
            }

            bodyNodes.Add(node);
        }

        AddChange(changes, currentChange, bodyNodes, pendingHeadings);
        return changes.Where(change => !string.IsNullOrWhiteSpace(change.Title)).ToList();
    }

    private void AddChange(
        List<PatchNotesChange> changes,
        PatchNotesChange change,
        IEnumerable<HtmlNode> bodyNodes,
        List<string> pendingHeadings)
    {
        if (change == null)
        {
            return;
        }

        var body = ToMarkdown(bodyNodes);
        if (string.IsNullOrWhiteSpace(body))
        {
            pendingHeadings.Add($"#### {change.Title}");
            return;
        }

        if (pendingHeadings.Any())
        {
            body = string.Join("\n\n", pendingHeadings) + "\n\n" + body;
            pendingHeadings.Clear();
        }

        change.Body = body;
        changes.Add(change);
    }

    private string GetFallbackTitle(HtmlNode content, string fallbackTitle)
    {
        var firstParagraph = content.Elements("p")
            .Select(p => NormalizeText(p.InnerText).TrimEnd(':'))
            .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));

        if (string.Equals(firstParagraph, "The following skins will be released in this patch", StringComparison.OrdinalIgnoreCase))
        {
            return "Skins";
        }

        if (string.Equals(firstParagraph, "The following chromas will be released this patch", StringComparison.OrdinalIgnoreCase))
        {
            return "Chromas";
        }

        return fallbackTitle;
    }

    private IEnumerable<HtmlNode> GetNodesAfter(HtmlNodeCollection nodes, HtmlNode titleNode)
    {
        var foundTitle = false;
        foreach (var node in nodes)
        {
            if (node == titleNode)
            {
                foundTitle = true;
                continue;
            }

            if (foundTitle)
            {
                yield return node;
            }
        }
    }

    private string GetChangeTitle(HtmlNode titleNode)
    {
        return NormalizeText((titleNode.QuerySelector("a") ?? titleNode).InnerText);
    }

    private string ToMarkdown(IEnumerable<HtmlNode> nodes)
    {
        var lines = nodes.Select(ToMarkdown)
            .Where(markdown => !string.IsNullOrWhiteSpace(markdown));

        return string.Join("\n\n", lines);
    }

    private string ToMarkdown(HtmlNode node)
    {
        return node.Name switch
        {
            "#text" => NormalizeText(node.InnerText),
            "blockquote" => ToBlockquoteMarkdown(node),
            "h4" => $"#### {NormalizeText(node.InnerText)}",
            "h5" => $"##### {NormalizeText(node.InnerText)}",
            "ul" => ToListMarkdown(node),
            "ol" => ToListMarkdown(node),
            "p" => NormalizeText(ToInlineMarkdown(node)),
            "hr" => "---",
            "div" => ToMarkdown(node.ChildNodes),
            _ => node.HasChildNodes ? ToMarkdown(node.ChildNodes) : NormalizeText(node.InnerText)
        };
    }

    private string ToBlockquoteMarkdown(HtmlNode node)
    {
        var text = ToMarkdown(node.ChildNodes);
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return string.Join("\n", text.Split('\n').Select(line => $"> {line}"));
    }

    private string ToListMarkdown(HtmlNode node)
    {
        var index = 1;
        var lines = new List<string>();
        foreach (var listItem in node.Elements("li"))
        {
            var prefix = node.Name == "ol" ? $"{index++}. " : "- ";
            lines.Add(prefix + NormalizeText(ToInlineMarkdown(listItem)));
        }

        return string.Join("\n", lines);
    }

    private string ToInlineMarkdown(HtmlNode node)
    {
        if (node.Name == "#text")
        {
            return HtmlEntity.DeEntitize(node.InnerText);
        }

        if (node.Name == "br")
        {
            return "\n";
        }

        if (node.Name == "img")
        {
            var src = node.GetAttributeValue("src", null);
            return string.IsNullOrWhiteSpace(src) ? "" : $"![]({src})";
        }

        var content = new StringBuilder();
        foreach (var childNode in node.ChildNodes)
        {
            content.Append(ToInlineMarkdown(childNode));
        }

        return node.Name switch
        {
            "strong" or "b" => $"**{content}**",
            "em" or "i" => $"_{content}_",
            _ => content.ToString()
        };
    }

    private string NormalizeText(string text)
    {
        return Regex.Replace(HtmlEntity.DeEntitize(text), @"\s+", " ").Trim();
    }
}
