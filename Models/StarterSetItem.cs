namespace LeagueOfItems.Models
{
    public class StarterSetItem
    {
        public int StarterSetId { get; set; }
        public StarterSetData StarterSet { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}