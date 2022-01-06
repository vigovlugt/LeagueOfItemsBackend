using System.Collections.Generic;

namespace LeagueOfItems.Domain.Models.Patches;

public class PatchNotesDataset
{
    public string Title { get; set; }
    public string Description { get; set; }
    
    public string BannerImageUrl { get; set; }
    public string HighlightImageUrl { get; set; }
    
    public string Quote { get; set; }

    public List<PatchNotesGroup> Groups { get; set; }
}