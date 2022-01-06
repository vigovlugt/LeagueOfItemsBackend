using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Patches;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Application.Patches.Services;

public class PatchNotesParserService
{
    private static Regex _stripRegex = new("[^a-z]");

    public static void PostProcess(PatchNotesDataset dataset, List<ChampionStats> champions, List<RuneStats> runes,
        List<ItemStats> items)
    {
        var dataByName = new Dictionary<string, (string, int)>();
        champions.ForEach(c => dataByName.Add(StripName(c.Name), ("CHAMPION", c.Id)));
        runes.ForEach(r => dataByName.Add(StripName(r.Name), ("RUNE", r.Id)));
        items.ForEach(i => dataByName.Add(StripName(i.Name), ("ITEM", i.Id)));

        foreach (var change in dataset.Groups.SelectMany(g => g.Changes))
        {
            var strippedName = StripName(change.Title);
            var exists = dataByName.TryGetValue(strippedName, out var data);
            if (!exists)
            {
                continue;
            }

            change.Type = data.Item1;
            change.Id = data.Item2;
        }

        foreach (var group in dataset.Groups)
        {
            group.Changes = group.Changes.Where(change => change.Id != null && change.Type != null).ToList();
        }

        dataset.Groups = dataset.Groups.Where(g => g.Changes.Any()).ToList();
    }

    private static string StripName(string name)
    {
        return _stripRegex.Replace(name.ToLower(), "");
    }
}