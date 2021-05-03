﻿using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Domain.Models.Champions
{
    public class ChampionStats : Champion
    {
        public int Wins { get; set; }
        public int Matches { get; set; }

        public List<ChampionRuneStats> RuneStats { get; set; }
        public List<ChampionItemStats> ItemStats { get; set; }
        public List<ChampionRoleStats> RoleStats { get; set; }
        public List<ChampionItemOrderStats> OrderStats { get; set; }

        public ChampionStats(Champion champion, List<ItemData> itemData, List<RuneData> runeData) : base(champion)
        {
            RuneData = runeData;
            ItemData = itemData;

            Wins = ChampionData.Sum(d => d.Wins);
            Matches = ChampionData.Sum(d => d.Matches);

            // Minimum of .5 procent pickrate
            var itemMatchMinimum = ItemData.Sum(i => i.Matches) * 0.005;
            var runeMatchMinimum = RuneData.Sum(i => i.Matches) * 0.005;
            var roleMatchMinimum = ChampionData.Sum(c => c.Matches) * 0.005;

            ItemStats = ItemData.GroupBy(i => i.ItemId)
                .Where(grouping => grouping.Sum(stats => stats.Matches) > itemMatchMinimum)
                .Select(grouping => new ChampionItemStats(grouping.Key, grouping.ToList()))
                .OrderByDescending(stats => stats.Matches)
                .ToList();

            RuneStats = RuneData.GroupBy(r => r.RuneId)
                .Where(grouping => grouping.Sum(stats => stats.Matches) > runeMatchMinimum)
                .Select(grouping => new ChampionRuneStats(grouping.Key, grouping.ToList()))
                .OrderByDescending(stats => stats.Matches)
                .ToList();

            RoleStats = ChampionData.GroupBy(c => c.Role)
                .Where(grouping => grouping.Sum(s => s.Matches) > roleMatchMinimum)
                .Select(grouping => new ChampionRoleStats(grouping.Key, grouping.ToList()))
                .OrderByDescending(stats => stats.Matches)
                .ToList();

            OrderStats = Enumerable.Range(0, 5).Select(i =>
            {
                var data = ItemData.Where(d => d.Order == i).ToList();
                var orderMatchesMinimum = data.Sum(d => d.Matches) * 0.005;

                var championItemStats = data
                    .GroupBy(d => d.ItemId)
                    .Where(grouping => grouping.Sum(d => d.Matches) > orderMatchesMinimum / 5)
                    .Select(grouping => new ChampionItemStats(grouping.Key, grouping.ToList()))
                    .OrderByDescending(stats => stats.Matches)
                    .ToList();

                return new ChampionItemOrderStats(i, championItemStats);
            }).ToList();
        }
    }
}