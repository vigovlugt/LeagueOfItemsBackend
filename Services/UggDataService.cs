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

            foreach (var champion in champions)
            {
                _logger.LogInformation("Downloading data for {Champion}", champion.Name);

                var (newStarterSetData, newItemData) = await GetUggItemData(champion.Id);

                starterSetData.AddRange(FilterItemData(newStarterSetData));
                itemData.AddRange(FilterItemData(newItemData));
            }

            await SaveStarterSetData(starterSetData);
            await SaveItemData(itemData);
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

                        for (var order = 0; order < uggItemDataByOrder.Count; order++)
                        {
                            var uggItemData = uggItemDataByOrder[order];
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
        private static (List<Models.Ugg.UggSimpleStarterSetData>, List<List<Models.Ugg.UggSimpleItemData>>) ParseItemData(
            List<List<JsonElement>> itemData)
        {
            var starterItemData = itemData[0];

            var itemDataByOrder = itemData.Skip(1).ToList();

            var newStarterItemData = starterItemData.Select(startSetInfo => new Models.Ugg.UggSimpleStarterSetData
            {
                ItemIds = startSetInfo[0].EnumerateArray().Select(el => el.GetInt32()).ToList(),
                Wins = startSetInfo[1].GetInt32(),
                Matches = startSetInfo[2].GetInt32()
            }).ToList();

            var newItemDataByOrder = itemDataByOrder.Select(itemsByOrder => itemsByOrder.Select(itemInfo =>
                new Models.Ugg.UggSimpleItemData
                {
                    ItemId = itemInfo[0].GetInt32(),
                    Wins = itemInfo[1].GetInt32(),
                    Matches = itemInfo[2].GetInt32()
                }).ToList()).ToList();

            return (newStarterItemData, newItemDataByOrder);
        }

        private static List<T> FilterItemData<T>(List<T> data) where T : IUggItemData
        {
            var regions = new List<UggRegion> {UggRegion.Euw1, UggRegion.Na1, UggRegion.Kr, UggRegion.World};
            var ranks = new List<UggRank> {UggRank.Challenger, UggRank.PlatinumPlus, UggRank.Overall};

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
    }
}