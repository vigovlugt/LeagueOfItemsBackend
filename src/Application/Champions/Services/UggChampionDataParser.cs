using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueOfItems.Application.Ugg.Services;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Ugg;

namespace LeagueOfItems.Application.Champions.Services;

public static class UggChampionDataParser
{
    public static async Task<List<ChampionData>> Parse(
        int championId,
        Stream stream,
        string version)
    {
        var parsed = await UggResponseParser.Parse<List<JsonElement>, ChampionData>(stream,
            (region, rank, role, data) =>
            {
                var uggChampionData = ParseChampionData(data);

                return new ChampionData
                {
                    Region = region,
                    Rank = rank,
                    Role = role,
                    ChampionId = championId,
                    Matches = uggChampionData.Matches,
                    Wins = uggChampionData.Wins,
                    Patch = version
                };
            });

        return parsed;
    }

    private static UggSimpleChampionData ParseChampionData(List<JsonElement> championData)
    {
        return new()
        {
            Wins = championData[0].GetInt32(),
            Matches = championData[1].GetInt32(),
        };
    }
}