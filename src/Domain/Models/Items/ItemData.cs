using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Domain.Models.Items
{
    public class ItemData : IData
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int Order { get; set; }

        public int ChampionId { get; set; }
        public Champion Champion { get; set; }
        
        public Region Region { get; set; }
        public Rank Rank { get; set; }
        public Role Role { get; set; }
        public string Patch { get; set; }
        public int Wins { get; set; }
        public int Matches { get; set; }
    }
}