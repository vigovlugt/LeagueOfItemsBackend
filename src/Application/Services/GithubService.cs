using System.Text.Json;
using System.Threading.Tasks;
using LeagueOfItems.Domain.Models;
using LeagueOfItems.Domain.Models.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Octokit;

namespace LeagueOfItems.Application.Services
{
    public interface IGithubService
    {
        Task StoreDataset();
    }

    public class GithubService : IGithubService
    {
        private readonly GitHubClient _client;
        private readonly IItemService _itemService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<GithubService> _logger;
        private readonly string _owner;
        private readonly string _path;

        private readonly string _repository;
        private readonly IRuneService _runeService;

        public GithubService(IConfiguration configuration, IRuneService runeService, IItemService itemService,
            ILogger<GithubService> logger)
        {
            _runeService = runeService;
            _itemService = itemService;
            _logger = logger;
            _client = new GitHubClient(new ProductHeaderValue("LeagueOfItems"))
            {
                Credentials = new Credentials(configuration["Github:Token"])
            };

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _repository = configuration["Github:Repository"];
            _owner = configuration["Github:Owner"];
            _path = configuration["Github:Path"];
        }


        public async Task StoreDataset()
        {
            _logger.LogInformation("Storing Dataset in Github");

            var json = await GetDatasetJson();
            var sha = await GetGithubDatasetSha();

            var updateFileRequest = new UpdateFileRequest("Update League of Items dataset", json, sha);
            await _client.Repository.Content.UpdateFile(_owner, _repository, _path, updateFileRequest);

            _logger.LogInformation("Dataset updated in Github");
        }

        private async Task<string> GetDatasetJson()
        {
            _logger.LogInformation("Getting Dataset as JSON");

            var itemStats = await _itemService.GetAllItems(new ItemFilter());
            var runeStats = await _runeService.GetAllRunes(new ItemFilter());

            var dataset = new ItemRuneDataset
            {
                Items = itemStats,
                Runes = runeStats
            };

            return JsonSerializer.Serialize(dataset, _jsonSerializerOptions);
        }

        private async Task<string> GetGithubDatasetSha()
        {
            var contents = await _client.Repository.Content.GetAllContents(_owner, _repository, _path);

            _logger.LogInformation("Got Dataset Github SHA {Sha}", contents[0].Sha);

            return contents[0].Sha;
        }
    }
}