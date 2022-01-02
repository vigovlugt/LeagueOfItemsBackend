using System.Collections.Generic;
using System.Text.Json.Serialization;
using LeagueOfItems.Domain.Models.BuildPaths;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Riot;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Domain.Models.Champions;

public class Champion
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Blurb { get; set; }
    public string Lore { get; set; }

    public string RiotId { get; set; }

    [JsonIgnore] public List<ItemData> ItemData { get; set; }
    [JsonIgnore] public List<RuneData> RuneData { get; set; }
    [JsonIgnore] public List<ChampionData> ChampionData { get; set; }
    [JsonIgnore] public List<BuildPathData> BuildPathData { get; set; }

    public Champion()
    {
    }

    public Champion(int id, string name, string title, string blurb, string lore, string riotId,
        List<ChampionData> championData)
    {
        Id = id;
        Name = name;
        Title = title;
        Blurb = blurb;
        Lore = lore;
        RiotId = riotId;
        ChampionData = championData;
    }

    public Champion(Champion champion) : this(champion.Id, champion.Name, champion.Title, champion.Blurb,
        champion.Lore, champion.RiotId, champion.ChampionData)
    {
    }

    public Champion(RiotChampion riotChampion) : this(0, riotChampion.Name, riotChampion.Title, riotChampion.Blurb,
        riotChampion.Lore,
        riotChampion.Id, null)
    {
        Id = int.Parse(riotChampion.Key);
    }
}

public class ChampionComparer : IEqualityComparer<Champion>
{
    public bool Equals(Champion a, Champion b)
    {
        return a.Id == b.Id;
    }

    public int GetHashCode(Champion c)
    {
        return c.Id.GetHashCode();
    }
}