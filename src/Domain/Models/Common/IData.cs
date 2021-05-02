namespace LeagueOfItems.Domain.Models.Common
{
    public interface IData
    {
        public int ChampionId { get; set; }
        public Region Region { get; set; }
        public Rank Rank { get; set; }
        public Role Role { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }
    }
}