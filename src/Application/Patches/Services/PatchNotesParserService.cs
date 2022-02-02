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

    /// <summary>
    /// Adds .Type and .Id to changes. Also converts details which should be changes to changes.
    /// </summary>
    /// <param name="dataset"></param>
    /// <param name="champions"></param>
    /// <param name="runes"></param>
    /// <param name="items"></param>
    public static void PostProcess(PatchNotesDataset dataset, List<ChampionStats> champions, List<RuneStats> runes,
        List<ItemStats> items)
    {
        var dataByName = new Dictionary<string, (string, int)>();
        champions.ForEach(c => dataByName.Add(StripName(c.Name), ("CHAMPION", c.Id)));
        runes.ForEach(r => dataByName.Add(StripName(r.Name), ("RUNE", r.Id)));
        items.ForEach(i => dataByName.Add(StripName(i.Name), ("ITEM", i.Id)));

        foreach (var group in dataset.Groups)
        {
            var newChanges = new List<PatchNotesChange>();
            foreach (var change in group.Changes)
            {
                foreach (var detail in change.Details)
                {
                    if (string.IsNullOrEmpty(detail.Title))
                    {
                        continue;
                    }
                    
                    // If detail is really a change, create a change from this detail.
                    var detailStrippedName = StripName(detail.Title);
                    var detailEntityExists = dataByName.TryGetValue(detailStrippedName, out var detailData);

                    if (detailEntityExists)
                    {
                        newChanges.Add(new PatchNotesChange
                        {
                            Type = detailData.Item1,
                            Id = detailData.Item2,
                            Title = detail.Title,
                            Details = new() {detail with {Title = null}},
                            Summary = "",
                            Quote = ""
                        });
                    }
                }

                var strippedName = StripName(change.Title);
                var exists = dataByName.TryGetValue(strippedName, out var data);
                if (!exists)
                {
                    continue;
                }

                change.Type = data.Item1;
                change.Id = data.Item2;
            }

            group.Changes.AddRange(newChanges);
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