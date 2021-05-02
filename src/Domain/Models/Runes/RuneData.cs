using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Domain.Models.Runes
{
    public class RuneData : IData
    {
        public int RuneId { get; set; }
        public Rune Rune { get; set; }

        public int Tier { get; set; }

        public int ChampionId { get; set; }
        public Champion Champion { get; set; }
        public Region Region { get; set; }
        public Rank Rank { get; set; }
        public Role Role { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }
    }
}