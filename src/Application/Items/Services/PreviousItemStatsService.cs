using System.Collections.Generic;
using System.Linq;
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

            stat.PreviousMatches = previousStat.Matches;
            stat.PreviousWins = previousStat.Wins;
        }
    }
}