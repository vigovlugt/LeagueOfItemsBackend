using System.Collections.Generic;
using LeagueOfItems.Models.Ugg;

namespace LeagueOfItems.Models
{
    public class StarterSetData : IItemData
    {
        public int Id { get; set; }
        public List<StarterSetItem> Items { get; set; }
        public int ChampionId { get; set; }
        public UggRegion Region { get; set; }
        public UggRank Rank { get; set; }
        public UggRole Role { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }
    }
}