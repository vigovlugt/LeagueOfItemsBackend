using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Domain.Models.Items;

public class ItemChampionStats : IStats
{
    public int ItemId { get; set; }
    public int ChampionId { get; set; }

    public int Wins { get; set; }
    public int Matches { get; set; }

    public ItemChampionStats(int itemId, int championId, List<ItemData> itemData)
    {
        ItemId = itemId;
        ChampionId = championId;

        Wins = itemData.Sum(d => d.Wins);
        Matches = itemData.Sum(d => d.Matches);
    }
}