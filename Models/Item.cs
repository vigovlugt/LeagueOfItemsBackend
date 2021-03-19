using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace LeagueOfItems.Models
{
  public class Item
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Colloq { get; set; }
    public string Plaintext { get; set; }
    public int Depth { get; set; }
    
    public List<ItemData> ItemData { get; set; }

    [NotMapped]
    public List<string> From { get; set; }
    [NotMapped]
    public ItemImage Image { get; set; }
    [NotMapped]
    public ItemGold Gold { get; set; }
    [NotMapped]
    public List<string> Tags { get; set; }
    [NotMapped]
    public Dictionary<int, bool> Maps { get; set; }
    [NotMapped]
    public Dictionary<string, float> Stats { get; set; }
    [NotMapped]
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