using System.Collections.Generic;

namespace LeagueOfItems.Models.Riot
{
    public class RiotChampionResponse
    {
        public string Type { get; set; }
        public string Format { get; set; }
        public string Version { get; set; }
        public Dictionary<string, RiotChampion> Data { get; set; }
    }
}