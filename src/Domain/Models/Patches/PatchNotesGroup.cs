using System.Collections.Generic;
using System.Linq;

namespace LeagueOfItems.Domain.Models.Patches;

public class PatchNotesGroup
{
    public string Id { get; set; }
    public string Title { get; set; }
    public List<PatchNotesChange> Changes { get; set; } = new();

    public PatchNotesGroup(string id, string title)
    {
        Id = id;
        Title = title;
    }
}