using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Items;

namespace LeagueOfItems.Domain.Models.Champions
{
    public class ChampionItemStats
    {
        public int ItemId { get; set; }
        public int Wins { get; set; }
        public int Matches { get; set; }

        public ChampionItemStats(int itemId, List<ItemData> itemData)
        {
            ItemId = itemId;

            Wins = itemData.Sum(d => d.Wins);
            Matches = itemData.Sum(d => d.Matches);
        }
    }
}