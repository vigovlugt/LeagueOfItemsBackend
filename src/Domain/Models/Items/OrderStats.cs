using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Champions;

namespace LeagueOfItems.Domain.Models.Items
{
    public class OrderStats
    {
        public int Order { get; set; }
        public List<ChampionStats> ChampionStats { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }

        public OrderStats(int order, List<ChampionStats> championStats)
        {
            Order = order;
            ChampionStats = championStats;
            Wins = championStats.Sum(d => d.Wins);
            Matches = championStats.Sum(d => d.Matches);
        }
    }
}