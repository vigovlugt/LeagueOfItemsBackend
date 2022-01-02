using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Domain.Models.Champions;

public class ChampionData : IData
{
    public int ChampionId { get; set; }
    public Champion Champion { get; set; }
    public Region Region { get; set; }
    public Rank Rank { get; set; }
    public Role Role { get; set; }
    public string Patch { get; set; }

    public int Wins { get; set; }
    public int Matches { get; set; }
}