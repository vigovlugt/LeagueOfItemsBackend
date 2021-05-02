using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Champions;

namespace LeagueOfItems.Domain.Models.Items
{
    public class ItemStats : Item
    {
        public int Wins { get; set; }
        public int Matches { get; set; }

        public List<ItemChampionStats> ChampionStats { get; set; }
        public List<ItemOrderStats> OrderStats { get; set; }

        public ItemStats(Item item) : base(item)
        {
            Wins = ItemData.Sum(d => d.Wins);
            Matches = ItemData.Sum(d => d.Matches);

            // Minimum of .5 procent pickrate
            var championMatchMinimum = Matches * 0.005;

            ChampionStats = ItemData
                .GroupBy(c => c.ChampionId)
                .Where(grouping => grouping.Sum(i => i.Matches) > championMatchMinimum)
                .Select(grouping => new ItemChampionStats(grouping.Key, grouping.ToList()))
                .OrderByDescending(stats => stats.Matches)
                .ToList();

            OrderStats = Enumerable.Range(0, 5).Select(i =>
            {
                var championStats = ItemData
                    .Where(itemData => itemData.Order == i)
                    .GroupBy(c => c.ChampionId)
                    .Where(grouping => grouping.Sum(itemData => itemData.Matches) > championMatchMinimum / 5)
                    .Select(grouping => new ItemChampionStats(grouping.Key, grouping.ToList()))
                    .OrderByDescending(stats => stats.Matches)
                    .ToList();

                return new ItemOrderStats(i, championStats);
            }).ToList();
        }
    }
}