using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Champions;

namespace LeagueOfItems.Application.Champions.Services
{
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
                
                var previousRoleById = previousStat.RoleStats.ToDictionary(s => s.Role);
                
                foreach (var roleStats in stat.RoleStats)
                {
                    var previousRoleStat = previousRoleById.GetValueOrDefault(roleStats.Role);
                    if (previousRoleStat == null)
                    {
                        continue;
                    }
                    
                    roleStats.PreviousMatches = previousRoleStat.Matches;
                    roleStats.PreviousWins = previousRoleStat.Wins;
                }

                stat.PreviousMatches = previousStat.Matches;
                stat.PreviousWins = previousStat.Wins;
            }
        }
    }
}