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
        Task<List<RiotItem>> GetItems();
        Task SaveItems(List<Item> items);
        Task<List<RiotChampion>> GetChampions();
        Task SaveChampions(List<Champion> champions);

        Task SaveAll();
        
        Task<List<RiotRunePath>> GetRunes();
        Task SaveRunes(List<RunePath> runePaths);
        Task<string> GetCurrentVersion();
        Task<List<string>> GetVersions();
    }

    public class RiotDataService : IRiotDataService
    {
        private readonly ILogger<RiotDataService> _logger;
        private readonly ItemContext _context;

        private readonly string _baseUrl = "https://ddragon.leagueoflegends.com/";
        private readonly HttpClient _client;

        private string _version;

        public RiotDataService(ILogger<RiotDataService> logger, IHttpClientFactory clientFactory, ItemContext context)
        {
            _logger = logger;
            _context = context;

            _client = clientFactory.CreateClient();
        }

        public async Task SaveAll()
        {
            var riotItems = await GetItems();

            var items = riotItems.Select(Item.FromRiotItem).ToList();

            await SaveItems(items);

            var riotChampions = await GetChampions();

            var champions = riotChampions.Select(Champion.FromRiotChampion).ToList();

            await SaveChampions(champions);

            var runes = await GetRunes();

            var runePaths = runes.Select(RunePath.FromRiotRunePath).ToList();

            await SaveRunes(runePaths);
        }

        public async Task<string> GetCurrentVersion()
        {
            if (_version != null)
            {
                return _version;
            }

            var response = await _client.GetAsync(_baseUrl + "api/versions.json");
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            var versions = await JsonSerializer.DeserializeAsync<List<string>>(responseStream);
            _version = versions[0];

            _logger.LogInformation("Current version {Version}", _version);

            return _version;
        }

        public async Task<List<string>> GetVersions()
        {
            var response = await _client.GetAsync(_baseUrl + "api/versions.json");
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            var versions = await JsonSerializer.DeserializeAsync<List<string>>(responseStream);

            return versions;
        }

        public async Task<List<RiotItem>> GetItems()
        {
            var version = await GetCurrentVersion();

            var response = await _client.GetAsync(_baseUrl + $"cdn/{version}/data/en_US/item.json");
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            var itemResponse = await JsonSerializer.DeserializeAsync<RiotItemResponse>(responseStream,
                new JsonSerializerOptions
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

        public async Task<List<RiotChampion>> GetChampions()
        {
            var version = await GetCurrentVersion();
            var response = await _client.GetAsync(_baseUrl + $"cdn/{version}/data/en_US/champion.json");
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            var championResponse = await JsonSerializer.DeserializeAsync<RiotChampionResponse>(responseStream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            _logger.LogInformation("{ChampionAmount} items found", championResponse.Data.Values.Count);

            return championResponse.Data.Values.ToList();
        }

        public async Task SaveChampions(List<Champion> champions)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM Champions;");

            _logger.LogInformation("{ChampionAmount} champions deleted", deleted);

            _context.Champions.AddRange(champions);

            await _context.SaveChangesAsync();

            _logger.LogInformation("{ChampionAmount} champions saved", champions.Count);
        }

        public async Task<List<RiotRunePath>> GetRunes()
        {
            var version = await GetCurrentVersion();
            var response = await _client.GetAsync(_baseUrl + $"cdn/{version}/data/en_US/runesReforged.json");
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            var runeResponse = await JsonSerializer.DeserializeAsync<List<RiotRunePath>>(responseStream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return runeResponse;
        }

        public async Task SaveRunes(List<RunePath> runePaths)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM RunePaths;");

            _logger.LogInformation("{RunePathAmount} rune paths deleted", deleted);

            _context.RunePaths.AddRange(runePaths);

            await _context.SaveChangesAsync();

            _logger.LogInformation("{RunePathAmount} rune paths saved", runePaths.Count);
        }
    }
}