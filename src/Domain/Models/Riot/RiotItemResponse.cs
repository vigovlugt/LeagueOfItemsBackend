using System.Collections.Generic;
using LeagueOfItems.Domain.Models.Runes;

namespace LeagueOfItems.Domain.Models.Riot;

public class RiotItemResponse
{
    public string Type { get; set; }
    public string Version { get; set; }
    public Basic Basic { get; set; }
    public Dictionary<int, RiotItem> Data { get; set; }
    public List<Group> Groups { get; set; }
    public List<Tree> Tree { get; set; }
}

public class Basic
{
    public string Name { get; set; }
    public Rune Rune { get; set; }
    public ItemGold ItemGold { get; set; }
    public string Group { get; set; }
    public string Description { get; set; }
    public string Colloq { get; set; }
    public string Plaintext { get; set; }
    public bool Consumed { get; set; }
    public int Stacks { get; set; }
    public int Depth { get; set; }
    public bool ConsumeOnFull { get; set; }
    public List<object> From { get; set; }
    public List<object> Into { get; set; }
    public int SpecialRecipe { get; set; }
    public bool InStore { get; set; }
    public bool HideFromAll { get; set; }
    public string RequiredChampion { get; set; }
    public string RequiredAlly { get; set; }
    public Dictionary<string, float> Stats { get; set; }
    public List<object> Tags { get; set; }
    public Dictionary<int, bool> Maps { get; set; }
}

public class Tree
{
    public string Header { get; set; }
    public List<string> Tags { get; set; }
}

public class Group
{
    public string Id { get; set; }
    public string MaxGroupOwnable { get; set; }
}