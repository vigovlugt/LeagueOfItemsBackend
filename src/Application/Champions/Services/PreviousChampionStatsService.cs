using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Champions;

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
            stat.PreviousMatches = previousStat.Matches;
            stat.PreviousWins = previousStat.Wins;
            stat.PreviousBans = previousStat.Bans;

            // Set role stats
            var previousRoleById = previousStat.RoleStats.ToDictionary(s => s.Role);
            foreach (var roleStats in stat.RoleStats)
            {
                var previousRoleStat = previousRoleById.GetValueOrDefault(roleStats.Role);

                roleStats.PreviousMatches = previousRoleStat?.Matches ?? 0;
                roleStats.PreviousWins = previousRoleStat?.Wins ?? 0;
                roleStats.PreviousBans = previousRoleStat?.Bans ?? 0;
            }

            // Set rune stats
            var previousRuneById = previousStat.RuneStats.ToDictionary(s => s.RuneId);
            foreach (var runeStats in stat.RuneStats)
            {
                var previousRoleStat = previousRuneById.GetValueOrDefault(runeStats.RuneId);

                runeStats.PreviousMatches = previousRoleStat?.Matches ?? 0;
                runeStats.PreviousWins = previousRoleStat?.Wins ?? 0;
            }

            // Set build path stats
            var previousBuildPathById =
                previousStat.BuildPathStats.ToDictionary(s => new {s.Item1Id, s.Item2Id, s.Item3Id});
            foreach (var buildPathStats in stat.BuildPathStats)
            {
                var previousBuildPathStats = previousBuildPathById.GetValueOrDefault(new {buildPathStats.Item1Id, buildPathStats.Item2Id, buildPathStats.Item3Id});

                buildPathStats.PreviousMatches = previousBuildPathStats?.Matches ?? 0;
                buildPathStats.PreviousWins = previousBuildPathStats?.Wins ?? 0;
            }
        }
    }
}