using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Common;
using LeagueOfItems.Domain.Models.Items;

namespace LeagueOfItems.Application.Builds
{
    public static class BuildAnalyzer
    {
        public static List<BuildStats> GetNewBuilds(List<ChampionStats> championStats,
            List<ChampionStats> previousChampionStats)
        {
            var list = new List<BuildStats>();

            foreach (var champion in championStats)
            {
                var previousChampion = previousChampionStats.Find(c => c.Id == champion.Id);
                if (previousChampion != null)
                {
                    list.AddRange(GetBuildsFromChampion(champion, previousChampion));
                }
            }

            return list;
        }

        private static List<BuildStats> GetBuildsFromChampion(ChampionStats championStats,
            ChampionStats previousChampionStats)
        {
            return GetItemBuilds(championStats, previousChampionStats)
                .Concat(GetRuneBuilds(championStats, previousChampionStats)).ToList();
        }

        private static List<BuildStats> GetItemBuilds(ChampionStats championStats,
            ChampionStats previousChampionStats)
        {
            var list = new List<BuildStats>();

            foreach (var orderStats in championStats.OrderStats)
            {
                var previousOrderStats = previousChampionStats.OrderStats.Find(o => o.Order == orderStats.Order);
                if (previousOrderStats == null)
                {
                    continue;
                }

                foreach (var itemStats in orderStats.ItemStats)
                {
                    var previousItemStats =
                        previousOrderStats.ItemStats.Find(o =>
                            o.ItemId == itemStats.ItemId && o.Matches >= 500);

                    if (itemStats.Matches < 500)
                    {
                        continue;
                    }

                    list.Add(new BuildStats(itemStats, previousItemStats, orderStats, previousOrderStats));
                }
            }

            return list;
        }

        private static List<BuildStats> GetRuneBuilds(ChampionStats championStats,
            ChampionStats previousChampionStats)
        {
            var list = new List<BuildStats>();
            foreach (var runeStats in championStats.RuneStats)
            {
                if (runeStats.Matches < 500)
                {
                    continue;
                }

                var previousRuneStats =
                    previousChampionStats.RuneStats.Find(o => o.RuneId == runeStats.RuneId && o.Matches >= 500);

                list.Add(new BuildStats(runeStats, previousRuneStats, championStats, previousChampionStats));
            }

            return list;
        }
    }
}