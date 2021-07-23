using System.Collections.Generic;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Domain.Models.Dataset
{
    public class BuildDataset
    {
        public List<BuildStats> PlayRateBuilds { get; set; }
        public List<BuildStats> WinRateBuilds { get; set; }
    }
}