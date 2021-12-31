using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueOfItems.Application.Ugg.Services;
using LeagueOfItems.Domain.Models.BuildPaths;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Common;
using LeagueOfItems.Domain.Models.Ugg;

namespace LeagueOfItems.Application.BuildPaths.Services
{
    public class UggBuildPathDataParser
    {
        public static async Task<List<BuildPathData>> Parse(
            int championId,
            Stream stream,
            string version)
        {
            var parsed = await UggResponseParser.Parse<List<JsonElement>, List<BuildPathData>>(stream,
                (region, rank, role, data) =>
                {
                    var uggBuildPathData = ParseBuildPathData(data);

                    return uggBuildPathData.Select(d => new BuildPathData
                    {
                        ChampionId = championId,
                        Item1Id = d.Item1Id,
                        Item2Id = d.Item2Id,
                        Item3Id = d.Item3Id,
                        Region = region,
                        Rank = rank,
                        Role = role,
                        Matches = d.Matches,
                        Wins = d.Wins,
                        Patch = version
                    }).ToList();
                });

            return parsed.SelectMany(x => x).ToList();
        }

        private static List<UggSimpleBuildPathData> ParseBuildPathData(List<JsonElement> data)
        {
            var numBuildPaths = data[6].GetArrayLength();

            return Enumerable.Range(0, numBuildPaths).Select(i => new UggSimpleBuildPathData
            {
                Item1Id = data[6][i][0][0].GetInt32(),
                Item2Id = data[6][i][0][1].GetInt32(),
                Item3Id = data[6][i][0][2].GetInt32(),
                Wins = data[6][i][1].GetInt32(),
                Matches = data[6][i][2].GetInt32(),
            }).ToList();
        }
    }
}