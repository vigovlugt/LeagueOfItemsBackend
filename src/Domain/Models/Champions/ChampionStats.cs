using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Domain.Models.Champions
{
    public class ChampionStats : Champion
    {
        public List<ChampionRuneStats> RuneStats { get; set; }
        public List<ChampionItemStats> ItemStats { get; set; }
        public List<ChampionRoleStats> RoleStats { get; set; }
        public List<ChampionItemOrderStats> OrderStats { get; set; }

        public ChampionStats(Champion champion, List<ItemData> itemData, List<RuneData> runeData) : base(champion)
        {
            RuneData = runeData;
            ItemData = itemData;

            // When this champion is played, rune/item/role must be picked at least 0.5%.
            var matchMinimum = Matches * Constants.MatchMinimum;

            ItemStats = ItemData.GroupBy(i => i.ItemId)
                .Where(grouping => grouping.Sum(stats => stats.Matches) > matchMinimum)
                .Select(grouping => new ChampionItemStats(grouping.Key, grouping.ToList()))
                .OrderByDescending(stats => stats.Matches)
                .ToList();

            RuneStats = RuneData.GroupBy(r => r.RuneId)
                .Where(grouping => grouping.Sum(stats => stats.Matches) > matchMinimum)
                .Select(grouping => new ChampionRuneStats(grouping.Key, grouping.ToList()))
                .OrderByDescending(stats => stats.Matches)
                .ToList();

            RoleStats = ChampionData.GroupBy(c => c.Role)
                .Where(grouping => grouping.Sum(s => s.Matches) > matchMinimum)
                .Select(grouping => new ChampionRoleStats(grouping.Key, grouping.ToList()))
                .OrderByDescending(stats => stats.Matches)
                .ToList();

            OrderStats = Enumerable.Range(0, 5).Select(i =>
            {
                var data = ItemData.Where(d => d.Order == i).ToList();
                var orderMatchesMinimum = data.Sum(d => d.Matches) * Constants.MatchMinimum;

                var championItemStats = data
                    .GroupBy(d => d.ItemId)
                    .Where(grouping => grouping.Sum(d => d.Matches) > orderMatchesMinimum)
                    .Select(grouping => new ChampionItemStats(grouping.Key, grouping.ToList()))
                    .OrderByDescending(stats => stats.Matches)
                    .ToList();

                return new ChampionItemOrderStats(i, championItemStats);
            }).ToList();
        }
    }
}