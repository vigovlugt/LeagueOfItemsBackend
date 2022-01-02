using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Champions;

namespace LeagueOfItems.Domain.Models.Items;

public class ItemOrderStats
{
    public int Order { get; set; }

    public int Wins { get; set; }
    public int Matches { get; set; }

    public List<ItemChampionStats> ChampionStats { get; set; }

    public ItemOrderStats(int order, List<ItemChampionStats> championStats)
    {
        Order = order;

        Wins = championStats.Sum(d => d.Wins);
        Matches = championStats.Sum(d => d.Matches);

        ChampionStats = championStats;
    }
}