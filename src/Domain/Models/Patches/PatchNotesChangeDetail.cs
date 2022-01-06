using System.Collections.Generic;

namespace LeagueOfItems.Domain.Models.Patches;

public class PatchNotesChangeDetail
{
    public string Title { get; set; }
    public List<PatchNotesAttributeChange> AttributeChanges { get; set; } = new();
}