using System.Collections.Generic;

namespace LeagueOfItems.Models.Ugg
{
    public class UggItemTableData
    {
        public int ChampionId { get; set; }

        public Dictionary<UggRegion, Dictionary<UggRank, Dictionary<UggRole, (List<UggStarterSetData>, List<List<UggItemData>>)>>> Data { get; set; }
    }
}