using System.Collections.Generic;

namespace LeagueOfItems.Domain.Models.Ugg
{
    public class UggItemTableData
    {
        public int ChampionId { get; set; }

        public Dictionary<UggRegion, Dictionary<UggRank,
            Dictionary<UggRole, (List<UggSimpleStarterSetData>, List<List<UggSimpleItemData>>)>>> Data { get; set; }
    }
}