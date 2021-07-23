using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Common;
using LeagueOfItems.Domain.Models.Items;

namespace LeagueOfItems.Domain.Models.Runes
{
    public class RuneChampionStats : IStats
    {
        public int RuneId { get; set; }
        public int ChampionId { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }

        public RuneChampionStats(int runeId, int championId, List<RuneData> runeData)
        {
            RuneId = runeId;
            ChampionId = championId;

            Wins = runeData.Sum(d => d.Wins);
            Matches = runeData.Sum(d => d.Matches);
        }
    }
}