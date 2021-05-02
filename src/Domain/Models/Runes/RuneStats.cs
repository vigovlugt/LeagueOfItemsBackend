using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Champions;

namespace LeagueOfItems.Domain.Models.Runes
{
    public class RuneStats : Rune
    {
        public int Wins { get; set; }
        public int Matches { get; set; }
        public List<RuneChampionStats> ChampionStats { get; set; }

        public RuneStats(Rune rune) : base(rune)
        {
            Wins = RuneData.Sum(d => d.Wins);
            Matches = RuneData.Sum(d => d.Matches);

            // Minimum of .5 percent pickrate
            var championMatchMinimum = Matches * 0.005;

            ChampionStats = RuneData
                .GroupBy(c => c.ChampionId)
                .Where(grouping => grouping.Sum(stats => stats.Matches) > championMatchMinimum)
                .Select(grouping => new RuneChampionStats(grouping.Key, grouping.ToList()))
                .OrderByDescending(stats => stats.Matches)
                .ToList();
        }
    }
}