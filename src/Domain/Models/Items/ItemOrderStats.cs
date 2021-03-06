using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Domain.Models.Items;

public class ItemOrderStats : IStats
{
    public int Order { get; set; }

    public int Wins { get; set; }
    public int Matches { get; set; }
    public int PreviousWins { get; set; }
    public int PreviousMatches { get; set; }

    public List<ItemChampionStats> ChampionStats { get; set; }

    public ItemOrderStats(int order, List<ItemChampionStats> championStats)
    {
        Order = order;

        Wins = championStats.Sum(d => d.Wins);
        Matches = championStats.Sum(d => d.Matches);

        ChampionStats = championStats;
    }
}