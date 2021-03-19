using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueOfItems.Models;
using LeagueOfItems.Models.Riot;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Services
{
    public interface IRiotDataService
    {
        Task<List<Item>> GetItems();
        Task SaveItems(List<Item> items);
        Task<List<Champion>> GetChampions();
        Task SaveChampions(List<Champion> champions);
    }

    public class RiotDataService : IRiotDataService
    {
        private readonly ILogger<RiotDataService> _logger;
        private readonly ItemContext _context;

        private readonly string _baseUrl = "https://ddragon.leagueoflegends.com/";
        private readonly HttpClient _client;

        public RiotDataService(ILogger<RiotDataService> logger, IHttpClientFactory clientFactory, ItemContext context)
        {
            _logger = logger;
            _context = context;

            _client = clientFactory.CreateClient();
        }

        private async Task<string> GetCurrentVersion()
        {
            var response = await _client.GetAsync(_baseUrl + "api/versions.json");
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            
            var versions = await JsonSerializer.DeserializeAsync<List<string>>(responseStream);
            var currentVersion = versions[0];
            
            _logger.LogInformation("Current version {Version}", currentVersion);

            return currentVersion;
        }

        public async Task<List<Item>> GetItems()
        {
            var version = await GetCurrentVersion();
            var response = await _client.GetAsync(_baseUrl + $"cdn/{version}/data/en_US/item.json");
            response.EnsureSuccessStatusCode();
            
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            
            var itemResponse = await JsonSerializer.DeserializeAsync<RiotItemResponse>(responseStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            foreach (var (id, item) in itemResponse.Data)
            {
                item.Id = id;
            }

            var items = itemResponse.Data.Values.ToList();
            
            _logger.LogInformation("{ItemAmount} items found", items.Count);

            return items;
        }

        public async Task SaveItems(List<Item> items)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM Items;");
            
            _logger.LogInformation("{ItemAmount} items deleted", deleted);
            
            _context.Items.AddRange(items);

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("{ItemAmount} items saved", items.Count);
        }

        public async Task<List<Champion>> GetChampions()
        {
            var version = await GetCurrentVersion();
            var response = await _client.GetAsync(_baseUrl + $"cdn/{version}/data/en_US/champion.json");
            response.EnsureSuccessStatusCode();
            
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            
            var championResponse = await JsonSerializer.DeserializeAsync<RiotChampionResponse>(responseStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var champions = championResponse.Data.Values.Select(Champion.FromRiotChampion).ToList();
            
            _logger.LogInformation("{ChampionAmount} items found", champions.Count);

            return champions;
        }
        
        public async Task SaveChampions(List<Champion> champions)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM Champions;");
            
            _logger.LogInformation("{ChampionAmount} champions deleted", deleted);
            
            _context.Champions.AddRange(champions);

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("{ChampionAmount} champions saved", champions.Count);
        }
    }
}