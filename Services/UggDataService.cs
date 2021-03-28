using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueOfItems.Models;
using LeagueOfItems.Models.Ugg;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Services
{
    public interface IUggDataService
    {
        public Task SaveForAllChampions();
    }

    public class UggDataService : IUggDataService
    {
        private readonly ILogger<UggDataService> _logger;
        private readonly IRiotDataService _riotDataService;

        private readonly string _baseUrl = "https://stats2.u.gg/";
        private readonly HttpClient _client;
        private readonly ItemContext _context;

        public UggDataService(ILogger<UggDataService> logger, IRiotDataService riotDataService,
            IHttpClientFactory clientFactory, ItemContext context)
        {
            _logger = logger;
            _riotDataService = riotDataService;
            _context = context;

            _client = clientFactory.CreateClient();
        }

        public async Task SaveForAllChampions()
        {
            var champions = await _context.Champions.ToListAsync();

            var starterSetData = new List<UggStarterSetData>();
            var itemData = new List<UggItemData>();
            var runeData = new List<UggRuneData>();

            await Task.WhenAll(champions.Select(async champion =>
            {
                _logger.LogInformation("Downloading data for {Champion}", champion.Name);

                var (newStarterSetData, newItemData) = await GetUggItemData(champion.Id);
                var newRuneData = await GetUggRuneData(champion.Id);

                starterSetData.AddRange(FilterItemData(newStarterSetData));
                itemData.AddRange(FilterItemData(newItemData));
                runeData.AddRange(FilterItemData(newRuneData));
            }));

            // await SaveStarterSetData(starterSetData);
            await SaveItemData(itemData);
            await SaveRuneData(runeData);
        }

        private async Task<(List<UggStarterSetData>, List<UggItemData>)> GetUggItemData(int championId)
        {
            var version = await _riotDataService.GetCurrentVersion();

            var uggVersion = string.Join('_', version.Split(".").Take(2));

            var response =
                await _client.GetAsync(_baseUrl +
                                       $"lol/1.1/table/items/{uggVersion}/ranked_solo_5x5/{championId}/1.4.0.json");
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            var itemData =
                await JsonSerializer
                    .DeserializeAsync<Dictionary<int, Dictionary<int, Dictionary<int, List<List<JsonElement>>>>>>(
                        responseStream);

            var parsedItemData = ParseUggData(championId, itemData);

            return parsedItemData;
        }

        private static (List<UggStarterSetData>, List<UggItemData>) ParseUggData(
            int championId,
            Dictionary<int, Dictionary<int, Dictionary<int, List<List<JsonElement>>>>> data)
        {
            var itemDataList = new List<UggItemData>();

            var starterSetData = new List<UggStarterSetData>();

            foreach (var (regionIndex, rankRoleData) in data)
            {
                var region = (UggRegion) regionIndex;

                foreach (var (rankIndex, roleData) in rankRoleData)
                {
                    var rank = (UggRank) rankIndex;

                    foreach (var (roleIndex, itemData) in roleData)
                    {
                        var role = (UggRole) roleIndex;

                        var (uggStarterSetData, uggItemDataByOrder) = ParseItemData(itemData);

                        starterSetData.AddRange(uggStarterSetData.Select(uggStarterSet => new UggStarterSetData
                        {
                            ChampionId = championId,
                            Items = uggStarterSet.ItemIds.GroupBy(id => id, (id, ids) => new UggStarterSetItem
                            {
                                ItemId = id,
                                Quantity = ids.ToList().Count
                            }).ToList(),
                            Matches = uggStarterSet.Matches,
                            Wins = uggStarterSet.Wins,
                            Rank = rank,
                            Region = region,
                            Role = role
                        }));

                        for (var i = 0; i < uggItemDataByOrder.Count; i++)
                        {
                            if (i == 1)
                            {
                                // TODO handle boots
                                continue;
                            }

                            var uggItemData = uggItemDataByOrder[i];

                            var order = i == 0 ? i : i - 1;

                            itemDataList.AddRange(uggItemData.Select(uggItem => new UggItemData
                            {
                                ChampionId = championId,
                                ItemId = uggItem.ItemId,
                                Matches = uggItem.Matches,
                                Wins = uggItem.Wins,
                                Order = order,
                                Rank = rank,
                                Region = region,
                                Role = role,
                            }));
                        }
                    }
                }
            }

            return (starterSetData, itemDataList);
        }

        // First list for starting 1,2,3,4,5th item
        // Second list for items in that category.
        private static (List<UggSimpleStarterSetData>, List<List<UggSimpleItemData>>)
            ParseItemData(
                List<List<JsonElement>> itemData)
        {
            var starterItemData = itemData[0];

            var itemDataByOrder = itemData.Skip(1).ToList();

            var newStarterItemData = starterItemData.Select(startSetInfo => new UggSimpleStarterSetData
            {
                ItemIds = startSetInfo[0].EnumerateArray().Select(el => el.GetInt32()).ToList(),
                Wins = startSetInfo[1].GetInt32(),
                Matches = startSetInfo[2].GetInt32()
            }).ToList();

            var newItemDataByOrder = itemDataByOrder.Select(itemsByOrder => itemsByOrder.Select(itemInfo =>
                new UggSimpleItemData
                {
                    ItemId = itemInfo[0].GetInt32(),
                    Wins = itemInfo[1].GetInt32(),
                    Matches = itemInfo[2].GetInt32()
                }).ToList()).ToList();

            return (newStarterItemData, newItemDataByOrder);
        }

        private static List<T> FilterItemData<T>(List<T> data) where T : IUggData
        {
            var regions = new List<UggRegion> {UggRegion.World}; //UggRegion.Euw1, UggRegion.Na1, UggRegion.Kr, 
            var ranks = new List<UggRank> {UggRank.PlatinumPlus}; // UggRank.Challenger, UggRank.Overall

            return data.Where(itemData => regions.Contains(itemData.Region) && ranks.Contains(itemData.Rank)).ToList();
        }

        private async Task SaveItemData(List<UggItemData> itemData)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM ItemData;");

            _logger.LogInformation("{ItemDataAmount} ItemData rows deleted", deleted);

            _context.ItemData.AddRange(itemData);

            await _context.SaveChangesAsync();

            _logger.LogInformation("{ItemDataAmount} ItemData rows saved", itemData.Count);
        }

        private async Task SaveStarterSetData(List<UggStarterSetData> starterSetData)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM StarterSetData;");

            _logger.LogInformation("{StarterSetDataAmount} StarterSets deleted", deleted);

            _context.StarterSetData.AddRange(starterSetData);

            await _context.SaveChangesAsync();

            _logger.LogInformation("{StarterSetDataAmount} StarterSets rows saved", starterSetData.Count);
        }

        private async Task<List<UggRuneData>> GetUggRuneData(int championId)
        {
            var version = await _riotDataService.GetCurrentVersion();

            var uggVersion = string.Join('_', version.Split(".").Take(2));

            var response = await _client.GetAsync(_baseUrl +
                                                  $"lol/1.1/table/runes/{uggVersion}/ranked_solo_5x5/{championId}/1.4.0.json");
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            var runeData =
                await JsonSerializer
                    .DeserializeAsync<Dictionary<int, Dictionary<int, Dictionary<int, List<Dictionary<int, List<int>>>>>
                    >>(
                        responseStream);

            var parsedRuneData = ParseUggRuneData(championId, runeData);

            return parsedRuneData;
        }

        private static List<UggRuneData> ParseUggRuneData(
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
                .ToList()
                .Select(pair => new UggSimpleRuneData(pair.Key, 0, pair.Value[0], pair.Value[1])));

            runeDataList.AddRange(primaryRuneData
                .ToList()
                .Select(pair => new UggSimpleRuneData(pair.Key, 0, pair.Value[0], pair.Value[1])));

            runeDataList.AddRange(secondaryRuneData
                .ToList()
                .Select(pair => new UggSimpleRuneData(pair.Key, 1, pair.Value[0], pair.Value[1])));

            return runeDataList;
        }

        private async Task SaveRuneData(List<UggRuneData> runeData)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM RuneData;");

            _logger.LogInformation("{RuneDataAmount} RuneData rows deleted", deleted);

            _context.RuneData.AddRange(runeData);

            await _context.SaveChangesAsync();

            _logger.LogInformation("{RuneDataAmount} RuneData rows saved", runeData.Count);
        }
    }
}