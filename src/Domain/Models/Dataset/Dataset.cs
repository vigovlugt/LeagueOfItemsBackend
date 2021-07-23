using System;
using System.Collections.Generic;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Common;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Domain.Models.Dataset
{
    public class Dataset
    {
        public List<ItemStats> Items { get; set; }
        public List<RuneStats> Runes { get; set; }
        public List<ChampionStats> Champions { get; set; }
        
        public List<BuildStats> Builds { get; set; }
        
        public DateTime Date { get; set; } = DateTime.Now;
        public string Version { get; set; }
    }
}