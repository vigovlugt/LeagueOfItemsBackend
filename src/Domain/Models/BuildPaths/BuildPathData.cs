using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Common;
using LeagueOfItems.Domain.Models.Items;

namespace LeagueOfItems.Domain.Models.BuildPaths
{
    public class BuildPathData : IData
    {
        public int ChampionId { get; set; }
        public Champion Champion { get; set; }
        public int Item1Id { get; set; }
        public Item Item1 { get; set; }
        public int Item2Id { get; set; }
        public Item Item2 { get; set; }
        public int Item3Id { get; set; }
        public Item Item3 { get; set; }
        
        public Region Region { get; set; }
        public Rank Rank { get; set; }
        public Role Role { get; set; }
        public string Patch { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }
    }
}