using System.Collections.Generic;
using System.Text.Json;

namespace LeagueOfItems.Application.Lolalytics.Models;

public class LolalyticsQwikChampionData
{
    public LolalyticsHeader Header { get; set; }
    public LolalyticsSummary Summary { get; set; }
    public LolalyticsNav Nav { get; set; }
    public List<List<JsonElement>> Skill6 { get; set; }
    public LolalyticsItemSets ItemSets { get; set; }
    public List<List<JsonElement>> Boots { get; set; }
    public List<List<JsonElement>> Item1 { get; set; }
    public List<List<JsonElement>> Item2 { get; set; }
    public List<List<JsonElement>> Item3 { get; set; }
    public List<List<JsonElement>> Item4 { get; set; }
    public List<List<JsonElement>> Item5 { get; set; }
}

public class LolalyticsHeader
{
    public string Lane { get; set; }
    public double Wr { get; set; }
    public int N { get; set; }
    public double Br { get; set; }
}

public class LolalyticsNav
{
    public LolalyticsLanes Lanes { get; set; }
}

public class LolalyticsLanes
{
    public double Top { get; set; }
    public double Jungle { get; set; }
    public double Middle { get; set; }
    public double Bottom { get; set; }
    public double Support { get; set; }
}

public class LolalyticsSummary
{
    public LolalyticsSummarySide Pick { get; set; }
    public LolalyticsSummarySide Win { get; set; }
}

public class LolalyticsSummarySide
{
    public LolalyticsSummaryRunes Runes { get; set; }
}

public class LolalyticsSummaryRunes
{
    public double Wr { get; set; }
    public int N { get; set; }
    public LolalyticsRuneSet Set { get; set; }
}

public class LolalyticsRuneSet
{
    public List<int> Pri { get; set; }
    public List<int> Sec { get; set; }
}

public class LolalyticsItemSets
{
    public Dictionary<string, List<int>> ItemBootSet3 { get; set; }
}
