using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Models;

namespace LeagueOfItems.ViewModels
{
    public class ItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Colloq { get; set; }
        public string Plaintext { get; set; }
        public int Depth { get; set; }
    
        public List<ItemData> ItemData { get; set; }

        public int? Matches { get; set; } = null;
        public int? Wins { get; set; } = null;
    }
}