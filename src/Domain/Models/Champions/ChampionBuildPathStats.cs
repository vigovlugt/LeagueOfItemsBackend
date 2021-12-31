using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.BuildPaths;

namespace LeagueOfItems.Domain.Models.Champions
{
    public class ChampionBuildPathStats
    {
        public int Item1Id { get; set; }
        public int Item2Id { get; set; }
        public int Item3Id { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }
        
        public int PreviousWins { get; set; }
        public int PreviousMatches { get; set; }
        
        public ChampionBuildPathStats(int item1Id, int item2Id, int item3Id, List<BuildPathData> buildPathData)
        {
            Item1Id = item1Id;
            Item2Id = item2Id;
            Item3Id = item3Id;
            Wins = buildPathData.Sum(d => d.Wins);
            Matches = buildPathData.Sum(d => d.Matches);
        }
    }
}