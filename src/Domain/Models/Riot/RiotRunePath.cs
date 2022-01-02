using System.Collections.Generic;

namespace LeagueOfItems.Domain.Models.Riot;

public class RiotRunePath
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Icon { get; set; }
    public string Name { get; set; }
    public List<RiotSlot> Slots { get; set; }
}