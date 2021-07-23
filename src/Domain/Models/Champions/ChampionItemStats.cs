using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Common;
using LeagueOfItems.Domain.Models.Items;

namespace LeagueOfItems.Domain.Models.Champions
{
    public class ChampionItemStats : IStats
    {
        public int ChampionId { get; set; }
        public int ItemId { get; set; }
        public int Wins { get; set; }
        public int Matches { get; set; }

        public ChampionItemStats(int championId, int itemId, List<ItemData> itemData)
        {
            ChampionId = championId;
            ItemId = itemId;

            Wins = itemData.Sum(d => d.Wins);
            Matches = itemData.Sum(d => d.Matches);
        }
    }
}