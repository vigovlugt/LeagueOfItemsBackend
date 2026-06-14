using System;
using System.Collections.Generic;
using System.Linq;

namespace LeagueOfItems.Application.Lolalytics.Services;

public static class LolalyticsRequestHelper
{
    public static string GetChampionSlug(string championId)
    {
        var slug = championId.ToLowerInvariant();

        return slug == "monkeyking" ? "wukong" : slug;
    }

    public static string GetPatch(string version)
    {
        return string.Join(".", version.Split(".").Take(2));
    }

    public static string ToQueryString(Dictionary<string, string> queryParams)
    {
        return string.Join("&", queryParams
            .Where(pair => !string.IsNullOrEmpty(pair.Value))
            .Select(pair => $"{pair.Key}={Uri.EscapeDataString(pair.Value)}"));
    }
}
