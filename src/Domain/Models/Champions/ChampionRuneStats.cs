using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Domain.Models.Champions
{
    public class ChampionRuneStats
    {
        public int RuneId { get; set; }
        public int Wins { get; set; }
        public int Matches { get; set; }

        public ChampionRuneStats(int runeId, List<RuneData> runeData)
        {
            RuneId = runeId;
            Wins = runeData.Sum(d => d.Wins);
            Matches = runeData.Sum(d => d.Matches);
        }
    }
}