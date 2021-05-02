using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LeagueOfItems.Application.Ugg.Services;
using LeagueOfItems.Domain.Models.Runes;
using LeagueOfItems.Domain.Models.Ugg;

namespace LeagueOfItems.Application.Runes.Services
{
    public static class UggRuneDataParser
    {
        public static async Task<List<RuneData>> Parse(
            int championId,
            Stream stream)
        {
            var parsed = await UggResponseParser.Parse<List<Dictionary<int, List<int>>>, List<RuneData>>(stream,
                (region, rank, role, data) =>
                {
                    var simpleRuneData = ParseRuneData(data);

                    return simpleRuneData.Select(uggSimpleRune => new RuneData
                    {
                        ChampionId = championId,
                        Rank = rank,
                        Role = role,
                        Region = region,
                        RuneId = uggSimpleRune.RuneId,
                        Matches = uggSimpleRune.Matches,
                        Wins = uggSimpleRune.Wins,
                        Tier = uggSimpleRune.Tier
                    }).ToList();
                });


            return parsed.SelectMany(x => x).ToList();
        }

        private static List<UggSimpleRuneData>
            ParseRuneData(
                List<Dictionary<int, List<int>>> runeData)
        {
            var runeDataList = new List<UggSimpleRuneData>();

            var keyStoneData = runeData[0];
            var primaryRuneData = runeData[1];
            var secondaryRuneData = runeData[2];

            runeDataList.AddRange(keyStoneData
                .Select(pair => new UggSimpleRuneData(pair.Key, 0, pair.Value[0], pair.Value[1])));

            runeDataList.AddRange(primaryRuneData
                .Select(pair => new UggSimpleRuneData(pair.Key, 0, pair.Value[0], pair.Value[1])));

            runeDataList.AddRange(secondaryRuneData
                .Select(pair => new UggSimpleRuneData(pair.Key, 1, pair.Value[0], pair.Value[1])));

            return runeDataList;
        }
    }
}