using System.Collections.Generic;
using System.Text.Json.Serialization;
using LeagueOfItems.Domain.Models.Riot;

namespace LeagueOfItems.Domain.Models.Items;

public class Item
{
    public Item()
    {
    }

    public Item(int id, string name, string description, string colloq, string plaintext, int depth,
        List<ItemData> itemData)
    {
        Id = id;
        Name = name;
        Description = description;
        Colloq = colloq;
        Plaintext = plaintext;
        Depth = depth;
        ItemData = itemData;
    }

    public Item(Item item) : this(item.Id, item.Name, item.Description, item.Colloq, item.Plaintext, item.Depth,
        item.ItemData)
    {
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Colloq { get; set; }
    public string Plaintext { get; set; }
    public int Depth { get; set; }

    [JsonIgnore] public List<ItemData> ItemData { get; set; } = new();

    public static Item FromRiotItem(RiotItem riotItem)
    {
        return new()
        {
            Id = riotItem.Id,
            Name = riotItem.Name,
            Description = riotItem.Description,
            Colloq = riotItem.Plaintext,
            Plaintext = riotItem.Plaintext,
            Depth = riotItem.Depth
        };
    }
}