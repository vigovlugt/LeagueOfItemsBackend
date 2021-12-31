using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Common;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Domain.Models.Champions
{
    public class ChampionRuneStats : IStats
    {
        public int ChampionId { get; set; }
        public int RuneId { get; set; }
        
        public int Wins { get; set; }
        public int Matches { get; set; }
        
        public int PreviousWins { get; set; }
        public int PreviousMatches { get; set; }

        public ChampionRuneStats(int championId, int runeId, List<RuneData> runeData)
        {
            ChampionId = championId;
            RuneId = runeId;
            Wins = runeData.Sum(d => d.Wins);
            Matches = runeData.Sum(d => d.Matches);
        }
    }
}