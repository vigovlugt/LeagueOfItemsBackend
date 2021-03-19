using LeagueOfItems.Models.Ugg;

namespace LeagueOfItems.Models
{
    public class ItemFilter
    {
        public UggRole Role { get; set; } = UggRole.None;
        public UggRegion Region { get; set; } = UggRegion.World;
        public UggRank Rank { get; set; } = UggRank.PlatinumPlus;
    }
}