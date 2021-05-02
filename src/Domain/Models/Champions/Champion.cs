using System.Collections.Generic;
using System.Text.Json.Serialization;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Riot;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Domain.Models.Champions
{
    public class Champion
    {
        public Champion()
        {
        }

        public Champion(int id, string name, string title, string blurb, string riotId, List<ChampionData> championData)
        {
            Id = id;
            Name = name;
            Title = title;
            Blurb = blurb;
            RiotId = riotId;
            ChampionData = championData;
        }

        public Champion(Champion champion) : this(champion.Id, champion.Name, champion.Title, champion.Blurb,
            champion.RiotId, champion.ChampionData)
        {
        }

        public Champion(RiotChampion riotChampion) : this(0, riotChampion.Name, riotChampion.Title, riotChampion.Blurb,
            riotChampion.Id, null)
        {
            Id = int.Parse(riotChampion.Key);
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Blurb { get; set; }

        public string RiotId { get; set; }

        [JsonIgnore] public List<ItemData> ItemData { get; set; }
        [JsonIgnore] public List<RuneData> RuneData { get; set; }
        [JsonIgnore] public List<ChampionData> ChampionData { get; set; }
    }
}