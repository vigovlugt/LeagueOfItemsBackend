using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Common;
using LeagueOfItems.Domain.Models.Items;

namespace LeagueOfItems.Application.Items.Services;

public class PreviousItemStatsService
{
    public static void SetPreviousItemStats(List<ItemStats> stats, List<ItemStats> previous)
    {
        var previousById = previous.ToDictionary(s => s.Id);

        foreach (var stat in stats)
        {
            var previousStat = previousById[stat.Id];
            if (previousStat == null)
            {
                continue;
            }

            ((IStats)stat).SetPreviousStats(previousStat);
            
            // Set champion stats
            var previousChampionById = previousStat.ChampionStats.ToDictionary(s => s.ChampionId);
            foreach (var championStats in stat.ChampionStats)
            {
                var previousChampionStats = previousChampionById.GetValueOrDefault(championStats.ChampionId);
                ((IStats)championStats).SetPreviousStats(previousChampionStats);
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

                var previousChampionOrderStatsById = previousOrderStats.ChampionStats.ToDictionary(s => s.ChampionId);
                foreach (var championOrderStat in orderStats.ChampionStats)
                {
                    var previousChampionOrderStats = previousChampionOrderStatsById.GetValueOrDefault(championOrderStat.ChampionId);
                    ((IStats)championOrderStat).SetPreviousStats(previousChampionOrderStats);
                }
            }
        }
    }
}