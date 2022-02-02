using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Application.Champions.Services;

public static class PreviousChampionStatsService
{
    public static void SetPreviousChampionStats(List<ChampionStats> stats, List<ChampionStats> previous)
    {
        var previousById = previous.ToDictionary(s => s.Id);

        foreach (var stat in stats)
        {
            var previousStat = previousById.GetValueOrDefault(stat.Id);
            if (previousStat == null)
            {
                continue;
            }
            
            // Set champion stats
            ((IStats)stat).SetPreviousStats(previousStat);
            stat.PreviousBans = previousStat.Bans;

            // Set role stats
            var previousRoleById = previousStat.RoleStats.ToDictionary(s => s.Role);
            foreach (var roleStats in stat.RoleStats)
            {
                var previousRoleStat = previousRoleById.GetValueOrDefault(roleStats.Role);

                ((IStats)roleStats).SetPreviousStats(previousRoleStat);
                roleStats.PreviousBans = previousRoleStat?.Bans ?? 0;
            }

            // Set rune stats
            var previousRuneById = previousStat.RuneStats.ToDictionary(s => s.RuneId);
            foreach (var runeStats in stat.RuneStats)
            {
                var previousRoleStat = previousRuneById.GetValueOrDefault(runeStats.RuneId);
                ((IStats)runeStats).SetPreviousStats(previousRoleStat);
            }
            
            // Set item stats
            var previousItemById = previousStat.ItemStats.ToDictionary(s => s.ItemId);
            foreach (var itemStats in stat.ItemStats)
            {
                var previousItemStat = previousItemById.GetValueOrDefault(itemStats.ItemId);
                ((IStats)itemStats).SetPreviousStats(previousItemStat);
            }

            // Set build path stats
            var previousBuildPathById =
                previousStat.BuildPathStats.ToDictionary(s => new {s.Item1Id, s.Item2Id, s.Item3Id});
            foreach (var buildPathStats in stat.BuildPathStats)
            {
                var previousBuildPathStats = previousBuildPathById.GetValueOrDefault(new {buildPathStats.Item1Id, buildPathStats.Item2Id, buildPathStats.Item3Id});
                ((IStats)buildPathStats).SetPreviousStats(previousBuildPathStats);
            }
            
            // Set order stats
            var previousOrderStatsById = previousStat.OrderStats.ToDictionary(s => s.Order);
            foreach (var orderStats in stat.OrderStats)
            {
                var previousOrderStats = previousOrderStatsById.GetValueOrDefault(orderStats.Order);
                if (previousOrderStats == null)
                {
                    continue;
                }
                
                ((IStats)orderStats).SetPreviousStats(previousOrderStats);

                var previousItemOrderStatsById = previousOrderStats.ItemStats.ToDictionary(s => s.ItemId);
                foreach (var itemOrderStat in orderStats.ItemStats)
                {
                    var previousItemOrderStats = previousItemOrderStatsById.GetValueOrDefault(itemOrderStat.ItemId);
                    ((IStats)itemOrderStat).SetPreviousStats(previousItemOrderStats);
                }
            }
        }
    }
}