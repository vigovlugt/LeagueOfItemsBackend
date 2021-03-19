using LeagueOfItems.Models.Ugg;

namespace LeagueOfItems.Models
{
    public interface IItemData
    {
        public int ChampionId { get; set; }
        public UggRegion Region { get; set; }
        public UggRank Rank { get; set; }
        public UggRole Role { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }
    }
}