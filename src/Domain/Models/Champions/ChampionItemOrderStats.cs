using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Domain.Models.Champions;

public class ChampionItemOrderStats : IStats
{
    public int Order { get; set; }

    public int Wins { get; set; }
    public int Matches { get; set; }
    public int PreviousWins { get; set; }
    public int PreviousMatches { get; set; }

    public List<ChampionItemStats> ItemStats { get; set; }

    public ChampionItemOrderStats(int order, List<ChampionItemStats> itemStats)
    {
        Order = order;

        Wins = itemStats.Sum(d => d.Wins);
        Matches = itemStats.Sum(d => d.Matches);

        ItemStats = itemStats;
    }
}