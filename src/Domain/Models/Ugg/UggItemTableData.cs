using System.Collections.Generic;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Domain.Models.Ugg;

public class UggItemTableData
{
    public int ChampionId { get; set; }

    public Dictionary<Region, Dictionary<Rank,
        Dictionary<Role, (List<UggSimpleStarterSetData>, List<List<UggSimpleItemData>>)>>> Data { get; set; }
}