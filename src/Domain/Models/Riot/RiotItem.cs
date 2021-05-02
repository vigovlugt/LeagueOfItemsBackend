using System.Collections.Generic;
using LeagueOfItems.Domain.Models.Items;

namespace LeagueOfItems.Domain.Models.Riot
{
    public class RiotItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Colloq { get; set; }
        public string Plaintext { get; set; }
        public int Depth { get; set; }

        public List<ItemData> ItemData { get; set; }

        public List<string> From { get; set; }
        public ItemImage Image { get; set; }
        public ItemGold Gold { get; set; }
        public List<string> Tags { get; set; }
        public Dictionary<int, bool> Maps { get; set; }
        public Dictionary<string, float> Stats { get; set; }
        public Dictionary<string, string> Effect { get; set; }
    }

    public class ItemImage
    {
        public string Full { get; set; }
        public string Sprite { get; set; }
        public string Group { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
    }

    public class ItemGold
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int Base { get; set; }
        public int Total { get; set; }
        public int Sell { get; set; }
        public bool Purchasable { get; set; }
    }
}