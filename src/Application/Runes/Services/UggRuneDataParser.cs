using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Ugg;

namespace LeagueOfItems.Application.Runes.Services
{
    public static class UggRuneDataParser
    {
        public static List<UggRuneData> Parse(
            int championId,
            Dictionary<int, Dictionary<int, Dictionary<int, List<Dictionary<int, List<int>>>>>> data)
        {
            var runeDataList = new List<UggRuneData>();

            foreach (var (regionIndex, rankRoleData) in data)
            {
                var region = (UggRegion) regionIndex;

                foreach (var (rankIndex, roleData) in rankRoleData)
                {
                    var rank = (UggRank) rankIndex;

                    foreach (var (roleIndex, itemData) in roleData)
                    {
                        var role = (UggRole) roleIndex;

                        var simpleRuneData = ParseRuneData(itemData);

                        runeDataList.AddRange(simpleRuneData.Select(uggSimpleRune => new UggRuneData
                        {
                            ChampionId = championId,
                            Rank = rank,
                            Role = role,
                            Region = region,
                            RuneId = uggSimpleRune.RuneId,
                            Matches = uggSimpleRune.Matches,
                            Wins = uggSimpleRune.Wins,
                            Tier = uggSimpleRune.Tier
                        }));
                    }
                }
            }

            return runeDataList;
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