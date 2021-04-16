using System.Collections.Generic;
using System.Linq;

namespace LeagueOfItems.Domain.Models
{
    public class RuneStats : Rune
    {
        public int Wins { get; set; }
        public int Matches { get; set; }
        public List<ChampionStats> ChampionStats { get; set; }

        public RuneStats(Rune rune) : base(rune)
        {
            Wins = RuneData.Select(d => d.Wins).Aggregate(0, (a, b) => a + b);
            Matches = RuneData.Select(d => d.Matches).Aggregate(0, (a, b) => a + b);

            // Minimum of .5 procent pickrate
            var championMatchMinimum = Matches * 0.005;

            ChampionStats = RuneData
                .GroupBy(c => c.ChampionId, (championId, runeData) =>
                    Models.ChampionStats.FromRuneStats(championId, runeData.ToList()))
                .Where(stats => stats.Matches > championMatchMinimum).OrderByDescending(stats => stats.Matches)
                .ToList();
        }
    }
}