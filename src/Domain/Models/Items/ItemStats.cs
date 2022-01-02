using System;
using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Domain.Models.Items;

public class ItemStats : Item, IStats
{
    public int Wins { get; set; }
    public int Matches { get; set; }
        
    public int PreviousWins { get; set; }
    public int PreviousMatches { get; set; }

    public List<ItemChampionStats> ChampionStats { get; set; }
    public List<ItemOrderStats> OrderStats { get; set; }

    public ItemStats(Item item) : base(item)
    {
        Wins = ItemData.Sum(d => d.Wins);
        Matches = ItemData.Sum(d => d.Matches);

        ChampionStats = ItemData
            .GroupBy(c => c.Champion, new ChampionComparer())
            .Where(grouping => grouping.Sum(i => i.Matches) >
                               Math.Max(
                                   grouping.Key.ChampionData.Sum(i => i.Matches) *
                                   Constants.MatchMinimumRelative, Constants.MinimumMatches))
            .Select(grouping => new ItemChampionStats(item.Id, grouping.Key.Id, grouping.ToList()))
            .OrderByDescending(stats => stats.Matches)
            .ToList();

        OrderStats = Enumerable.Range(0, 5).Select(i =>
        {
            var championStats = ItemData
                .Where(itemData => itemData.Order == i)
                .GroupBy(c => c.Champion, new ChampionComparer())
                .Where(grouping =>
                    grouping.Sum(d => d.Matches) > Math.Max(grouping.Key.ChampionData.Sum(d => d.Matches) *
                                                            Constants.MatchMinimumRelative,
                        Constants.MinimumMatches))
                .Select(grouping => new ItemChampionStats(item.Id, grouping.Key.Id, grouping.ToList()))
                .OrderByDescending(stats => stats.Matches)
                .ToList();

            return new ItemOrderStats(i, championStats);
        }).ToList();
    }
}