using System.Collections.Generic;
using System.Linq;

namespace LeagueOfItems.Domain.Models.Items
{
    public class ItemChampionStats
    {
        public int ChampionId { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }

        public ItemChampionStats(int championId, List<ItemData> itemData)
        {
            ChampionId = championId;

            Wins = itemData.Sum(d => d.Wins);
            Matches = itemData.Sum(d => d.Matches);
        }
    }
}