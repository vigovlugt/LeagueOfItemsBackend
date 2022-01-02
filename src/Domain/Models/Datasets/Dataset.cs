using System;
using System.Collections.Generic;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Common;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Patches;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Domain.Models.Datasets;

public class Dataset
{
    public List<ItemStats> Items { get; set; }
    public List<RuneStats> Runes { get; set; }
    public List<ChampionStats> Champions { get; set; }

    public int ChampionMatches { get; set; }
    public int PreviousChampionMatches { get; set; }

    public List<ScheduledPatch> PatchSchedule { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;
    public string Version { get; set; }
}