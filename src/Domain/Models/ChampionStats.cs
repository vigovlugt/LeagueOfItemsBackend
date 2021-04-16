using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Ugg;

namespace LeagueOfItems.Domain.Models
{
    public class ChampionStats
    {
        public int ChampionId { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }

        public static ChampionStats FromItemStats(int championId, List<UggItemData> itemData)
        {
            return new()
            {
                ChampionId = championId,
                Wins = itemData.Select(d => d.Wins).Aggregate(0, (a, b) => a + b),
                Matches = itemData.Select(d => d.Matches).Aggregate(0, (a, b) => a + b)
            };
        }
        
        public static ChampionStats FromRuneStats(int championId, List<UggRuneData> runeData)
        {
            return new()
            {
                ChampionId = championId,
                Wins = runeData.Select(d => d.Wins).Aggregate(0, (a, b) => a + b),
                Matches = runeData.Select(d => d.Matches).Aggregate(0, (a, b) => a + b)
            };
        }
    }
}