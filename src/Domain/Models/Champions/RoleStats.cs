using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Domain.Models.Champions;

public class RoleStats
{
    public Role Role { get; set; }

    public int Wins { get; set; }
    public int Matches { get; set; }

    public RoleStats(Role role, List<ChampionData> championData)
    {
        Role = role;
        Wins = championData.Sum(d => d.Wins);
        Matches = championData.Sum(d => d.Matches);
    }
}