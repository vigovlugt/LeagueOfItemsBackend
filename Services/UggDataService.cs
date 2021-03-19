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

        private readonly string _baseUrl = "https://stats2.u.gg/";
        private readonly HttpClient _client;
        private readonly ItemContext _context;

        public UggDataService(ILogger<UggDataService> logger, IHttpClientFactory clientFactory, ItemContext context)
        {
            _logger = logger;
            _context = context;

            _client = clientFactory.CreateClient();
        }

        public async Task SaveForAllChampions()
        {
            var champions = await _context.Champions.ToListAsync();

            var starterSetData = new List<StarterSetData>();
            var itemData = new List<ItemData>();

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

        public async Task<(List<StarterSetData>, List<ItemData>)> GetUggItemData(int championId)
        {
            var response = await _client.GetAsync(_baseUrl + $"lol/1.1/table/items/11_5/ranked_solo_5x5/{championId}/1.4.0.json");
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            var itemData =
                await JsonSerializer
                    .DeserializeAsync<Dictionary<int, Dictionary<int, Dictionary<int, List<List<JsonElement>>>>>>(
                        responseStream);

            var parsedItemData = ParseUggData(championId, itemData);

            var data = UggDataToItemData(parsedItemData);

            return data;
        }

        private UggItemTableData ParseUggData(
            int championId,
            Dictionary<int, Dictionary<int, Dictionary<int, List<List<JsonElement>>>>> data)
        {
            var newData = new UggItemTableData
            {
                ChampionId = championId,
                Data =
                    new Dictionary<UggRegion, Dictionary<UggRank,
                        Dictionary<UggRole, (List<UggStarterSetData>, List<List<UggItemData>>)>>>()
            };
                

            foreach (var (regionIndex, rankRoleData) in data)
            {
                var region = (UggRegion) regionIndex;
                newData.Data[region] = new Dictionary<UggRank, Dictionary<UggRole, (List<UggStarterSetData>, List<List<UggItemData>>)>>();

                foreach (var (rankIndex, roleData) in rankRoleData)
                {
                    var rank = (UggRank) rankIndex;
                    newData.Data[region][rank] = new Dictionary<UggRole, (List<UggStarterSetData>, List<List<UggItemData>>)>();

                    foreach (var (roleIndex, itemData) in roleData)
                    {
                        var role = (UggRole) roleIndex;

                        var newItemData = ParseItemData(itemData);

                        newData.Data[region][rank][role] = newItemData;
                    }
                }
            }
            
            return newData;
        }

        // First list for starting 1,2,3,4,5th item
        // Second list for items in that category.
        private static (List<UggStarterSetData>, List<List<UggItemData>>) ParseItemData(List<List<JsonElement>> itemData)
        {
            var starterItemData = itemData[0];
            
            var itemDataByOrder = itemData.Skip(1).ToList();

            var newStarterItemData = starterItemData.Select(startSetInfo => new UggStarterSetData
            {
                ItemIds = startSetInfo[0].EnumerateArray().Select(el => el.GetInt32()).ToList(),
                Wins = startSetInfo[1].GetInt32(),
                Matches = startSetInfo[2].GetInt32()
            }).ToList();

            var newItemDataByOrder = itemDataByOrder.Select(itemsByOrder => itemsByOrder.Select(itemInfo =>
                new UggItemData
                {
                    ItemId = itemInfo[0].GetInt32(),
                    Wins = itemInfo[1].GetInt32(),
                    Matches = itemInfo[2].GetInt32()
                }).ToList()).ToList();

            return (newStarterItemData, newItemDataByOrder);
        }

        private static (List<StarterSetData>, List<ItemData>) UggDataToItemData(UggItemTableData itemTableData)
        {
            var itemData = new List<ItemData>();

            var starterSetData = new List<StarterSetData>();
            
            foreach (var (region, rankRoleData) in itemTableData.Data)
            {
                foreach (var (rank, roleData) in rankRoleData)
                {
                    foreach (var (role, (uggStarterSetData, uggItemDataByOrder)) in roleData)
                    {
                        starterSetData.AddRange(uggStarterSetData.Select(uggStarterSet => new StarterSetData
                        {
                            ChampionId = itemTableData.ChampionId,
                            Items = uggStarterSet.ItemIds.GroupBy(id => id, (id, ids) => new StarterSetItem
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
                            itemData.AddRange(uggItemData.Select(uggItem => new ItemData
                            {
                                ChampionId = itemTableData.ChampionId,
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

            return (starterSetData, itemData);
        }

        private static List<T> FilterItemData<T>(List<T> data) where T : IItemData
        {
            var regions = new List<UggRegion> {UggRegion.Euw1, UggRegion.Na1, UggRegion.Kr, UggRegion.World};
            var ranks = new List<UggRank> {UggRank.Challenger, UggRank.PlatinumPlus, UggRank.Overall};
            
            return data.Where(itemData => regions.Contains(itemData.Region) && ranks.Contains(itemData.Rank) ).ToList();
        }

        private async Task SaveItemData(List<ItemData> itemData)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM ItemData;");
            
            _logger.LogInformation("{ItemDataAmount} ItemData rows deleted", deleted);
            
            _context.ItemData.AddRange(itemData);

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("{ItemDataAmount} ItemData rows saved", itemData.Count);
        }

        private async Task SaveStarterSetData(List<StarterSetData> starterSetData)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM StarterSetData;");
            
            _logger.LogInformation("{StarterSetDataAmount} StarterSets deleted", deleted);
            
            _context.StarterSetData.AddRange(starterSetData);

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("{StarterSetDataAmount} StarterSets rows saved", starterSetData.Count);
        }
    }
}