using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Champions;

namespace LeagueOfItems.Application.Champions.Services
{
    public static class PreviousChampionStats
    {
        public static void SetPreviousChampionStats(List<ChampionStats> stats, List<ChampionStats> previous)
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
}