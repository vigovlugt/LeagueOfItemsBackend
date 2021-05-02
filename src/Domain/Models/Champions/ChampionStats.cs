using System.Collections.Generic;
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

        public ChampionStats(Champion champion) : base(champion)
        {
            Wins = ItemData.Sum(d => d.Wins);
            Matches = ItemData.Sum(d => d.Matches);
        }

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
        }
    }
}