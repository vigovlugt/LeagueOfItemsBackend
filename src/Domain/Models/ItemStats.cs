using System.Collections.Generic;
using System.Linq;

namespace LeagueOfItems.Domain.Models
{
    public class ItemStats : Item
    {
        public int Wins { get; set; }
        public int Matches { get; set; }
        public List<ChampionStats> ChampionStats { get; set; }

        public List<OrderStats> OrderStats { get; set; }

        public ItemStats(Item item) : base(item)
        {
            Wins = ItemData.Select(d => d.Wins).Aggregate(0, (a, b) => a + b);
            Matches = ItemData.Select(d => d.Matches).Aggregate(0, (a, b) => a + b);

            // Minimum of .5 procent pickrate
            var championMatchMinimum = Matches * 0.005;

            ChampionStats = ItemData
                .GroupBy(c => c.ChampionId, (championId, itemData) =>
                    Models.ChampionStats.FromItemStats(championId, itemData.ToList()))
                .Where(stats => stats.Matches > championMatchMinimum).OrderByDescending(stats => stats.Matches)
                .ToList();

            OrderStats = ItemData
                .GroupBy(c => c.Order)
                .Select(group =>
                {
                    var championStats = group
                        .GroupBy(c => c.ChampionId, (championId, itemData) =>
                            Models.ChampionStats.FromItemStats(championId, itemData.ToList()))
                        .Where(stats => stats.Matches > championMatchMinimum / 5)
                        .OrderByDescending(stats => stats.Matches)
                        .ToList();

                    return Models.OrderStats.FromChampionStats(group.Key, championStats);
                }).ToList();

            for (var i = 0; i < 5; i++)
            {
                if (OrderStats.Any(o => o.Order == i))
                    continue;

                OrderStats.Add(new OrderStats
                {
                    ChampionStats = new List<ChampionStats>(),
                    Order = i
                });
            }

            OrderStats = OrderStats.OrderBy(o => o.Order).ToList();
        }
    }
}