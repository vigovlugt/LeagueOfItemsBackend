﻿using System.Collections.Generic;

namespace LeagueOfItems.Models.Ugg
{
    public class UggStarterSetData : IUggData
    {
        public int Id { get; set; }
        public List<UggStarterSetItem> Items { get; set; }
        public int ChampionId { get; set; }
        public UggRegion Region { get; set; }
        public UggRank Rank { get; set; }
        public UggRole Role { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }
    }
}