using LeagueOfItems.Models.Riot;

namespace LeagueOfItems.Models
{
    public class Champion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Blurb { get; set; }
        
        public string RiotId { get; set; }

        public static Champion FromRiotChampion(RiotChampion riotChampion)
        {
            return new()
            {
                Id = int.Parse(riotChampion.Key),
                Name = riotChampion.Name,
                Title = riotChampion.Title,
                Blurb = riotChampion.Blurb,
                RiotId = riotChampion.Id
            };
        }
    }
}