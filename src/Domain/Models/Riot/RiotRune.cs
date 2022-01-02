using System.Collections.Generic;

namespace LeagueOfItems.Domain.Models.Riot;

public class RiotRune
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Icon { get; set; }
    public string Name { get; set; }
    public string ShortDesc { get; set; }
    public string LongDesc { get; set; }
}

public class RiotSlot
{
    public List<RiotRune> Runes { get; set; }
}