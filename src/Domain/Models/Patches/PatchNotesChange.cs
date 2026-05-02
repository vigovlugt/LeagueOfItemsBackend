namespace LeagueOfItems.Domain.Models.Patches;

public class PatchNotesChange
{
    public string Type { get; set; }
    public int? Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}