namespace LeagueOfItems.Domain.Models.Ugg
{
    public class UggStarterSetItem
    {
        public int StarterSetId { get; set; }
        public UggStarterSetData UggStarterSet { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}