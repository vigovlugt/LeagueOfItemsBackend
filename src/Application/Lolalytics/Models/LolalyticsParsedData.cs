using System.Collections.Generic;
using LeagueOfItems.Domain.Models.BuildPaths;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Application.Lolalytics.Models;

public class LolalyticsParsedData
{
    public List<ChampionData> ChampionData { get; set; } = new();
    public List<ItemData> ItemData { get; set; } = new();
    public List<RuneData> RuneData { get; set; } = new();
    public List<BuildPathData> BuildPathData { get; set; } = new();

    public void Add(LolalyticsParsedData data)
    {
        ChampionData.AddRange(data.ChampionData);
        ItemData.AddRange(data.ItemData);
        RuneData.AddRange(data.RuneData);
        BuildPathData.AddRange(data.BuildPathData);
    }
}
