using System;
using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Migrations;

namespace LeagueOfItems.Models
{
    public class ItemStats : Item
    {
        public int Wins { get; set; }
        public int Matches { get; set; }
        public List<ChampionStats> ChampionStats { get; set; }

        public ItemStats(Item item) : base(item)
        {
            Wins = ItemData.Select(d => d.Wins).Aggregate(0, (a, b) => a + b);
            Matches = ItemData.Select(d => d.Matches).Aggregate(0, (a, b) => a + b);

            var championMatchMinimum = Matches * 0.005;

            ChampionStats = ItemData.GroupBy(c => c.ChampionId, (championId, itemData) =>
            {
                var itemDataList = itemData.ToList();
                return new ChampionStats
                {
                    ChampionId = championId,
                    Wins = itemDataList.Select(d => d.Wins).Aggregate(0, (a, b) => a + b),
                    Matches = itemDataList.Select(d => d.Matches).Aggregate(0, (a, b) => a + b)
                };
            }).Where(stats => stats.Matches > championMatchMinimum).OrderByDescending(stats => stats.Matches).ToList();
        }
    }
}