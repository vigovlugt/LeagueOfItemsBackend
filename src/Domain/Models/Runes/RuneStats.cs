using System;
using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Domain.Models.Runes
{
    public class RuneStats : Rune, IStats
    {
        public int Wins { get; set; }
        public int Matches { get; set; }
        public List<RuneChampionStats> ChampionStats { get; set; }

        public RuneStats(Rune rune) : base(rune)
        {
            Wins = RuneData.Sum(d => d.Wins);
            Matches = RuneData.Sum(d => d.Matches);

            ChampionStats = RuneData
                .GroupBy(c => c.Champion, new ChampionComparer())
                .Where(grouping => grouping.Sum(stats => stats.Matches) > Math.Max(
                    grouping.Key.ChampionData.Sum(c => c.Matches) * Constants.MatchMinimumRelative, Constants.MinimumMatches))
                .Select(grouping => new RuneChampionStats(rune.Id, grouping.Key.Id, grouping.ToList()))
                .OrderByDescending(stats => stats.Matches)
                .ToList();
        }
    }
}