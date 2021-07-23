using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Common;
using LeagueOfItems.Domain.Models.Dataset;
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

        public static BuildDataset CreateBuildDataset(List<BuildStats> buildStats)
        {
            var buildStatsByPlayrateIncrease = buildStats.OrderByDescending(b => b.PlayRate - b.PreviousPlayRate).ToList();
            var buildStatsByWinrateIncrease = buildStats
                .OrderByDescending(b => b.Wins / b.Matches - b.PreviousWins / b.PreviousMatches).ToList();

            return new BuildDataset
            {
                PlayRateBuilds = buildStatsByPlayrateIncrease,
                WinRateBuilds = buildStatsByWinrateIncrease
            };
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
                    var previousItemStats = previousOrderStats.ItemStats.Find(o => o.ItemId == itemStats.ItemId);
                    if (previousItemStats != null)
                    {
                        if (itemStats.Matches < 1000)
                        {
                            continue;
                        }
                        
                        list.Add(new BuildStats(itemStats, previousItemStats, orderStats, previousOrderStats));
                    }
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
                var previousRuneStats = previousChampionStats.RuneStats.Find(o => o.RuneId == runeStats.RuneId);
                if (previousRuneStats == null)
                {
                    continue;
                }
                
                if (runeStats.Matches < 1000)
                {
                    continue;
                }

                list.Add(new BuildStats(runeStats, previousRuneStats, championStats, previousChampionStats));
            }

            return list;
        }
    }
}