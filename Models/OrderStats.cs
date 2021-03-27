using System.Collections.Generic;
using System.Linq;

namespace LeagueOfItems.Models
{
    public class OrderStats
    {
        public int Order { get; set; }
        public List<ChampionStats> ChampionStats { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }

        public static OrderStats FromChampionStats(int order, List<ChampionStats> championStats)
        {
            return new()
            {
                Order = order,
                ChampionStats = championStats,
                Wins = championStats.Select(d => d.Wins).Aggregate(0, (a, b) => a + b),
                Matches = championStats.Select(d => d.Matches).Aggregate(0, (a, b) => a + b)
            };
        }
    }
}