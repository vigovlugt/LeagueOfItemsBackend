namespace LeagueOfItems.Domain.Models.Ugg
{
    public class UggItemData : IUggData
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int Order { get; set; }

        public int ChampionId { get; set; }
        public UggRegion Region { get; set; }
        public UggRank Rank { get; set; }
        public UggRole Role { get; set; }
        public int Wins { get; set; }
        public int Matches { get; set; }
    }
}