using System.Collections.Generic;
using System.Linq;

namespace LeagueOfItems.Domain.Models.Patches;

public class PatchNotesChange
{
    public string Type { get; set; }
    public int? Id { get; set; }
    public string Title { get; set; }
    public string Quote { get; set; }
    public List<PatchNotesChangeDetail> Details { get; set; } = new();

    public void AddAttributeChange(PatchNotesAttributeChange change)
    {
        if (!Details.Any())
        {
            Details.Add(new PatchNotesChangeDetail());
        }

        Details.Last().AttributeChanges.Add(change);
    }
}