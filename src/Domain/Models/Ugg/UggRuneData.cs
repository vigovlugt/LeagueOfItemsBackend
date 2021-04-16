namespace LeagueOfItems.Domain.Models.Ugg
{
    public class UggRuneData : IUggData
    {
        public int RuneId { get; set; }
        public Rune Rune { get; set; }

        public int ChampionId { get; set; }
        public UggRegion Region { get; set; }
        public UggRank Rank { get; set; }
        public UggRole Role { get; set; }

        public int Tier { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }
    }
}