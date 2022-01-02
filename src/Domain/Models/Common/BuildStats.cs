using System;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Domain.Models.Common;

public class BuildStats
{
    public int ChampionId { get; set; }

    public string BuildType { get; set; }
    public int BuildTypeId { get; set; }

    // Order is not used with rune builds.
    public int? Order { get; set; }

    public int Wins { get; set; }
    public int Matches { get; set; }
    public float PlayRate { get; set; }

    public int PreviousWins { get; set; }
    public int PreviousMatches { get; set; }
    public float PreviousPlayRate { get; set; }

    // TotalStats for runes will be champion stats, for items will be order stats.
    public BuildStats(IStats currentStats, IStats previousStats, IStats totalStats, IStats previousTotalStats)
    {
        Wins = currentStats.Wins;
        Matches = currentStats.Matches;
        PlayRate = currentStats.Matches / (float) totalStats.Matches;

        if (previousStats != null)
        {
            PreviousWins = previousStats.Wins;
            PreviousMatches = previousStats.Matches;
            PreviousPlayRate = previousStats.Matches / (float) previousTotalStats.Matches;
        }

        if (currentStats.GetType() == typeof(ChampionRuneStats))
        {
            BuildType = Common.BuildType.Rune;
            ChampionId = ((ChampionRuneStats) currentStats).ChampionId;
            BuildTypeId = ((ChampionRuneStats) currentStats).RuneId;
        }
        else if (currentStats.GetType() == typeof(ChampionItemStats))
        {
            BuildType = Common.BuildType.Item;
            ChampionId = ((ChampionItemStats) currentStats).ChampionId;
            BuildTypeId = ((ChampionItemStats) currentStats).ItemId;
            Order = ((ChampionItemOrderStats) totalStats).Order;
        }
        else
        {
            throw new Exception("BuildChampionStats can only be used with items or runes");
        }
    }
}

public static class BuildType
{
    public const string Item = "ITEM";

    public const string Rune = "RUNE";
}