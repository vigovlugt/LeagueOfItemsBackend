using System;
using System.Collections.Generic;

namespace LeagueOfItems.Domain.Models
{
    public class ItemRuneDataset
    {
        public List<ItemStats> Items { get; set; }
        public List<RuneStats> Runes { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Version { get; set; }
    }
}