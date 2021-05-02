using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using LeagueOfItems.Domain.Models.Riot;

namespace LeagueOfItems.Domain.Models.Runes
{
    public class RunePath
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore] public List<Rune> Runes { get; set; }

        public static RunePath FromRiotRunePath(RiotRunePath riotRunePath)
        {
            return new()
            {
                Id = riotRunePath.Id,
                Name = riotRunePath.Name,
                Runes = riotRunePath.Slots
                    .Select((slot, tier) => slot.Runes.Select(rune => Rune.FromRiotRune(tier, rune)))
                    .SelectMany(x => x)
                    .ToList()
            };
        }
    }
}