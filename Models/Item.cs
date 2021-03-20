using System.Collections.Generic;
using System.Text.Json.Serialization;
using LeagueOfItems.Models.Riot;
using LeagueOfItems.Models.Ugg;

namespace LeagueOfItems.Models
{
    public class Item
    {
        public Item()
        {
        }

        public Item(int id, string name, string description, string colloq, string plaintext, int depth,
            List<UggItemData> itemData)
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

        [JsonIgnore] public List<UggItemData> ItemData { get; set; }

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
}