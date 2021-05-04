using System.Collections.Generic;
using System.Text.Json.Serialization;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Riot;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Domain.Models.Champions
{
    public class Champion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Blurb { get; set; }
        public string Lore { get; set; }

        public string RiotId { get; set; }

        public int Wins { get; set; }
        public int Matches { get; set; }

        [JsonIgnore] public List<ItemData> ItemData { get; set; }
        [JsonIgnore] public List<RuneData> RuneData { get; set; }
        [JsonIgnore] public List<ChampionData> ChampionData { get; set; }

        public Champion()
        {
        }

        public Champion(int id, string name, string title, string blurb, string lore, string riotId,
            List<ChampionData> championData,
            int wins, int matches)
        {
            Id = id;
            Name = name;
            Title = title;
            Blurb = blurb;
            Lore = lore;
            RiotId = riotId;
            ChampionData = championData;
            Wins = wins;
            Matches = matches;
        }

        public Champion(Champion champion) : this(champion.Id, champion.Name, champion.Title, champion.Blurb,
            champion.Lore, champion.RiotId, champion.ChampionData, champion.Wins, champion.Matches)
        {
        }

        public Champion(RiotChampion riotChampion) : this(0, riotChampion.Name, riotChampion.Title, riotChampion.Blurb,
            riotChampion.Lore,
            riotChampion.Id, null, 0, 0)
        {
            Id = int.Parse(riotChampion.Key);
        }
    }
}