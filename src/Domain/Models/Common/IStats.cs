namespace LeagueOfItems.Domain.Models.Common;

public interface IStats
{
    public int Wins { get; set; }
    public int Matches { get; set; }
    public int PreviousWins { get; set; }
    public int PreviousMatches { get; set; }

    public void SetPreviousStats(IStats previousStats)
    {
        if (previousStats == null)
        {
            return;
        }

        PreviousMatches = previousStats.Matches;
        PreviousWins = previousStats.Wins;
    }
}