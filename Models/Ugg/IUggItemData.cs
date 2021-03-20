namespace LeagueOfItems.Models.Ugg
{
    public interface IUggItemData
    {
        public int ChampionId { get; set; }
        public UggRegion Region { get; set; }
        public UggRank Rank { get; set; }
        public UggRole Role { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }
    }
}