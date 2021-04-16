namespace LeagueOfItems.Domain.Models.Ugg
{
    public class UggSimpleRuneData
    {
        public UggSimpleRuneData(int runeId, int tier, int wins, int matches)
        {
            RuneId = runeId;
            Tier = tier;
            Wins = wins;
            Matches = matches;
        }
        
        public int RuneId { get; set; }
        
        public int Tier { get; set; }
        
        public int Wins { get; set; }
        public int Matches { get; set; }
    }
}