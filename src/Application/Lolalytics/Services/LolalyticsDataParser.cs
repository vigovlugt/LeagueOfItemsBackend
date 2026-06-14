using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using LeagueOfItems.Application.Lolalytics.Models;
using LeagueOfItems.Domain.Models.BuildPaths;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Common;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Application.Lolalytics.Services;

public static class LolalyticsDataParser
{
    private static readonly Region WorldRegion = Region.World;
    private static readonly Rank EmeraldPlusRank = Rank.EmeraldPlus;

    public static LolalyticsParsedData Parse(
        int championId,
        string version,
        string role,
        LolalyticsQwikChampionData data,
        HashSet<int> itemIds,
        HashSet<int> runeIds)
    {
        var parsed = new LolalyticsParsedData();
        if (data?.Header == null || data.Header.N <= 0)
        {
            return parsed;
        }

        var domainRole = LolalyticsRoleMapper.ToDomainRole(role);
        parsed.ChampionData.Add(ParseChampionData(championId, version, domainRole, data.Header));
        parsed.ItemData.AddRange(ParseItemData(championId, version, domainRole, data, itemIds));
        parsed.RuneData.AddRange(ParseRuneData(championId, version, domainRole, data, runeIds));
        parsed.BuildPathData.AddRange(ParseBuildPathData(championId, version, domainRole, data, itemIds));

        return parsed;
    }

    private static ChampionData ParseChampionData(
        int championId,
        string version,
        Role role,
        LolalyticsHeader header)
    {
        return new ChampionData
        {
            ChampionId = championId,
            Region = WorldRegion,
            Rank = EmeraldPlusRank,
            Role = role,
            Patch = version,
            Matches = header.N,
            Wins = GetWins(header.N, header.Wr),
            Bans = GetWins(header.N, header.Br)
        };
    }

    private static List<ItemData> ParseItemData(
        int championId,
        string version,
        Role role,
        LolalyticsQwikChampionData data,
        HashSet<int> itemIds)
    {
        var itemData = new List<ItemData>();

        itemData.AddRange(ParseItemRows(championId, version, role, data.Item1, 0, ItemSlot.Core, itemIds));
        itemData.AddRange(ParseItemRows(championId, version, role, data.Item2, 1, ItemSlot.Core, itemIds));
        itemData.AddRange(ParseItemRows(championId, version, role, data.Item3, 2, ItemSlot.Core, itemIds));
        itemData.AddRange(ParseItemRows(championId, version, role, data.Item4, 3, ItemSlot.Core, itemIds));
        itemData.AddRange(ParseItemRows(championId, version, role, data.Item5, 4, ItemSlot.Core, itemIds));
        itemData.AddRange(ParseItemRows(championId, version, role, data.Boots, 0, ItemSlot.Boots, itemIds));

        return itemData
            .GroupBy(d => new {d.ItemId, d.ChampionId, d.Rank, d.Order, d.Region, d.Role, d.Patch})
            .Select(grouping => grouping.OrderByDescending(d => d.Matches).First())
            .ToList();
    }

    private static IEnumerable<ItemData> ParseItemRows(
        int championId,
        string version,
        Role role,
        IEnumerable<List<JsonElement>> rows,
        int order,
        ItemSlot slot,
        HashSet<int> itemIds)
    {
        if (rows == null)
        {
            return new List<ItemData>();
        }

        return rows
            .Where(row => row.Count >= 4)
            .Select(row => new
            {
                ItemId = GetInt(row[0]),
                WinRate = GetDouble(row[1]),
                Matches = GetInt(row[3])
            })
            .Where(row => row.Matches > 0 && itemIds.Contains(row.ItemId))
            .Select(row => new ItemData
            {
                ChampionId = championId,
                ItemId = row.ItemId,
                Matches = row.Matches,
                Wins = GetWins(row.Matches, row.WinRate),
                Order = order,
                Slot = slot,
                Rank = EmeraldPlusRank,
                Region = WorldRegion,
                Role = role,
                Patch = version
            })
            .ToList();
    }

    private static List<RuneData> ParseRuneData(
        int championId,
        string version,
        Role role,
        LolalyticsQwikChampionData data,
        HashSet<int> runeIds)
    {
        var runeData = new List<RuneData>();
        var runes = data.Summary?.Pick?.Runes;
        if (runes?.Set == null || runes.N <= 0)
        {
            return runeData;
        }

        var wins = GetWins(runes.N, runes.Wr);
        runeData.AddRange(ParseRuneSet(championId, version, role, runes.Set.Pri, 0, runes.N, wins, runeIds));
        runeData.AddRange(ParseRuneSet(championId, version, role, runes.Set.Sec, 1, runes.N, wins, runeIds));

        return runeData
            .GroupBy(d => new {d.RuneId, d.ChampionId, d.Rank, d.Tier, d.Region, d.Role, d.Patch})
            .Select(grouping => grouping.First())
            .ToList();
    }

    private static IEnumerable<RuneData> ParseRuneSet(
        int championId,
        string version,
        Role role,
        IEnumerable<int> runeSet,
        int tier,
        int matches,
        int wins,
        HashSet<int> runeIds)
    {
        if (runeSet == null)
        {
            return new List<RuneData>();
        }

        return runeSet
            .Where(runeIds.Contains)
            .Select(runeId => new RuneData
            {
                ChampionId = championId,
                Rank = EmeraldPlusRank,
                Role = role,
                Region = WorldRegion,
                RuneId = runeId,
                Matches = matches,
                Wins = wins,
                Tier = tier,
                Patch = version
            })
            .ToList();
    }

    private static List<BuildPathData> ParseBuildPathData(
        int championId,
        string version,
        Role role,
        LolalyticsQwikChampionData data,
        HashSet<int> itemIds)
    {
        var itemBootSet3 = data.ItemSets?.ItemBootSet3;
        if (itemBootSet3 == null)
        {
            return new List<BuildPathData>();
        }

        return itemBootSet3
            .Select(pair => new
            {
                Items = ParseBuildPathItems(pair.Key),
                Stats = pair.Value
            })
            .Where(row => row.Items.Count == 3 && row.Stats.Count >= 2 && row.Items.All(itemIds.Contains))
            .Select(row => new BuildPathData
            {
                ChampionId = championId,
                Item1Id = row.Items[0],
                Item2Id = row.Items[1],
                Item3Id = row.Items[2],
                Region = WorldRegion,
                Rank = EmeraldPlusRank,
                Role = role,
                Matches = row.Stats[0],
                Wins = row.Stats[1],
                Patch = version
            })
            .Where(row => row.Matches > 0)
            .GroupBy(d => new
            {
                d.ChampionId,
                d.Item1Id,
                d.Item2Id,
                d.Item3Id,
                d.Rank,
                d.Region,
                d.Role,
                d.Patch
            })
            .Select(grouping => grouping.First())
            .ToList();
    }

    private static List<int> ParseBuildPathItems(string key)
    {
        return key.Split("_")
            .Select(part => int.TryParse(part, out var itemId) ? itemId : 0)
            .Where(itemId => itemId > 0)
            .ToList();
    }

    private static int GetWins(int matches, double winRate)
    {
        return (int)Math.Round(matches * winRate / 100, MidpointRounding.AwayFromZero);
    }

    private static int GetInt(JsonElement element)
    {
        return (int)Math.Round(GetDouble(element), MidpointRounding.AwayFromZero);
    }

    private static double GetDouble(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.String => double.TryParse(element.GetString(), NumberStyles.Float,
                CultureInfo.InvariantCulture, out var value)
                ? value
                : 0,
            _ => 0
        };
    }
}
