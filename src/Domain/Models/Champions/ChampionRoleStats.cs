using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Common;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Domain.Models.Champions;

public class ChampionRoleStats: IStats
{
    public Role Role { get; set; }
    public int Wins { get; set; }
    public int Bans { get; set; }
    public int Matches { get; set; }
    public int PreviousWins { get; set; }
    public int PreviousBans { get; set; }
    public int PreviousMatches { get; set; }

    public ChampionRoleStats(Role role, List<ChampionData> championData)
    {
        Role = role;
        Wins = championData.Sum(d => d.Wins);
        Bans = championData.Sum(d => d.Bans);
        Matches = championData.Sum(d => d.Matches);
    }
}