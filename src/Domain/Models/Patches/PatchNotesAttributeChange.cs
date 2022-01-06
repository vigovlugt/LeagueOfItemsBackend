namespace LeagueOfItems.Domain.Models.Patches;

public class PatchNotesAttributeChange
{
    public string Attribute { get; set; }
    public string ChangeType { get; set; }
    public string Before { get; set; }
    public string After { get; set; }
    public string Removed { get; set; }
}