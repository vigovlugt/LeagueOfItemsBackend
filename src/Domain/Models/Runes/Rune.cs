using System.Collections.Generic;
using System.Text.Json.Serialization;
using LeagueOfItems.Domain.Models.Riot;

namespace LeagueOfItems.Domain.Models.Runes
{
    public class Rune
    {
        public Rune()
        {
        }

        public Rune(int id,
            string name,
            int tier,
            string shortDescription,
            string longDescription,
            int runePathId,
            List<RuneData> runeData)
        {
            Id = id;
            Name = name;
            Tier = tier;
            ShortDescription = shortDescription;
            LongDescription = longDescription;
            RunePathId = runePathId;
            RuneData = runeData;
        }

        public Rune(Rune rune) : this(rune.Id, rune.Name, rune.Tier, rune.ShortDescription, rune.LongDescription,
            rune.RunePathId,
            rune.RuneData)
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Tier { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }

        public int RunePathId { get; set; }
        public RunePath RunePath { get; set; }

        [JsonIgnore] public List<RuneData> RuneData { get; set; }

        public static Rune FromRiotRune(int tier, RiotRune riotRune)
        {
            return new()
            {
                Id = riotRune.Id,
                Name = riotRune.Name,
                Tier = tier,
                ShortDescription = riotRune.ShortDesc,
                LongDescription = riotRune.LongDesc
            };
        }
    }
}