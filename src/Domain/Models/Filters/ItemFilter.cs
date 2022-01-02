using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Domain.Models.Filters;

public class ItemFilter
{
    public Role Role { get; set; } = Role.None;
    public Region Region { get; set; } = Region.World;
    public Rank Rank { get; set; } = Rank.PlatinumPlus;
}