using System;
using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.BuildPaths;
using LeagueOfItems.Domain.Models.Common;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Domain.Models.Champions;

public class ChampionStats : Champion, IStats
{
    public int Wins { get; set; }
    public int Bans { get; set; }
    public int Matches { get; set; }

    public int PreviousWins { get; set; }
    public int PreviousBans { get; set; }
    public int PreviousMatches { get; set; }

    public List<ChampionRuneStats> RuneStats { get; set; }
    public List<ChampionItemStats> ItemStats { get; set; }
    public List<ChampionRoleStats> RoleStats { get; set; }
    public List<ChampionBuildPathStats> BuildPathStats { get; set; }
    public List<ChampionItemOrderStats> OrderStats { get; set; }

    public ChampionStats(Champion champion, List<ItemData> itemData, List<RuneData> runeData,
        List<BuildPathData> buildPathData) : base(champion)
    {
        Wins = ChampionData.Sum(c => c.Wins);
        Matches = ChampionData.Sum(c => c.Matches);
        Bans = ChampionData.FirstOrDefault()?.Bans ?? 0;

        RuneData = runeData;
        ItemData = itemData;
        BuildPathData = buildPathData;

        // When this champion is played, rune/item/role must be picked at least 0.5%.
        var matchMinimum = Math.Max(Matches * Constants.MatchMinimumRelative, Constants.MinimumMatches);

        ItemStats = ItemData.GroupBy(i => i.ItemId)
            .Where(grouping => grouping.Sum(stats => stats.Matches) > matchMinimum)
            .Select(grouping => new ChampionItemStats(champion.Id, grouping.Key, grouping.ToList()))
            .OrderByDescending(stats => stats.Matches)
            .ToList();

        RuneStats = RuneData.GroupBy(r => r.RuneId)
            .Where(grouping => grouping.Sum(stats => stats.Matches) > matchMinimum)
            .Select(grouping => new ChampionRuneStats(champion.Id, grouping.Key, grouping.ToList()))
            .OrderByDescending(stats => stats.Matches)
            .ToList();

        RoleStats = ChampionData.GroupBy(c => c.Role)
            .Where(grouping => grouping.Sum(s => s.Matches) > matchMinimum)
            .Select(grouping => new ChampionRoleStats(grouping.Key, grouping.ToList()))
            .OrderByDescending(stats => stats.Matches)
            .ToList();


        var buildPathMatchMinimum = Math.Max(buildPathData.Sum(d => d.Matches) * Constants.MatchMinimumRelative,
            Constants.MinimumMatches);
        BuildPathStats = BuildPathData.GroupBy(c => new {c.Item1Id, c.Item2Id, c.Item3Id})
            .Where(grouping => grouping.Sum(s => s.Matches) > buildPathMatchMinimum)
            .Select(grouping => new ChampionBuildPathStats(grouping.Key.Item1Id, grouping.Key.Item2Id,
                grouping.Key.Item3Id, grouping.ToList()))
            .OrderByDescending(stats => stats.Matches)
            .ToList();

        OrderStats = Enumerable.Range(0, 5).Select(i =>
        {
            var data = ItemData.Where(d => d.Order == i).ToList();
            var orderMatchesMinimum = Math.Max(data.Sum(d => d.Matches) * Constants.MatchMinimumRelative,
                Constants.MinimumMatches);

            var championItemStats = data
                .GroupBy(d => d.ItemId)
                .Where(grouping => grouping.Sum(d => d.Matches) > orderMatchesMinimum)
                .Select(grouping => new ChampionItemStats(champion.Id, grouping.Key, grouping.ToList()))
                .OrderByDescending(stats => stats.Matches)
                .ToList();

            return new ChampionItemOrderStats(i, championItemStats);
        }).ToList();
    }
}