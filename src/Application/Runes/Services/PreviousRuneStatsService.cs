using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Application.Runes.Services;

public class PreviousRuneStatsService
{
    public static void SetPreviousRuneStats(List<RuneStats> stats, List<RuneStats> previous)
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