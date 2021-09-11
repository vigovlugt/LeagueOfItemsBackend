using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Builds;
using LeagueOfItems.Application.Champions.Queries;
using LeagueOfItems.Application.Champions.Services;
using LeagueOfItems.Application.Items.Queries;
using LeagueOfItems.Application.Runes.Queries;
using LeagueOfItems.Application.Ugg.Helpers;
using LeagueOfItems.Application.Ugg.Queries;
using LeagueOfItems.Domain.Models;
using LeagueOfItems.Domain.Models.Dataset;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Octokit;

namespace LeagueOfItems.Application.Github.Commands
{
    public record UploadGithubCommand : MediatR.IRequest
    {
    }

    public class UploadGithubCommandHandler : IRequestHandler<UploadGithubCommand>
    {
        private readonly ILogger<UploadGithubCommandHandler> _logger;
        private readonly GitHubClient _client;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        private readonly string _repository;
        private readonly string _owner;
        private readonly string _path;
        private readonly string _fileName;

        private readonly IMediator _mediator;

        public UploadGithubCommandHandler(ILogger<UploadGithubCommandHandler> logger,
            IConfiguration configuration, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;

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
            _fileName = configuration["Github:FileName"];
        }

        public async Task<Unit> Handle(UploadGithubCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Uploading dataset to frontend Github");

            var json = await GetDatasetJson();
            var sha = await GetGithubDatasetSha();

            await UploadDataset(json, sha);

            _logger.LogInformation("Dataset uploaded");

            return Unit.Value;
        }

        private async Task UploadDataset(string json, string sha)
        {
            var updateFileRequest = new UpdateFileRequest("Update League of Items dataset", json, sha);

            await _client.Repository.Content.UpdateFile(_owner, _repository, Path.Join(_path, _fileName),
                updateFileRequest);
        }

        private async Task<string> GetDatasetJson()
        {
            _logger.LogInformation("Getting Dataset as JSON");
            var patch = await _mediator.Send(new GetUggVersionQuery());
            var previousPatch = UggVersionHelper.GetPreviousVersion(patch);

            _logger.LogInformation("Getting all Items");
            var itemStats = await _mediator.Send(new GetAllItemsQuery(patch));

            _logger.LogInformation("Getting all Runes");
            var runeStats = await _mediator.Send(new GetAllRunesQuery(patch));

            _logger.LogInformation("Getting all Champions");
            var championStats = await _mediator.Send(new GetAllChampionsQuery(patch));
            var previousChampionStats = await _mediator.Send(new GetAllChampionsQuery(previousPatch));
            PreviousChampionStats.SetPreviousChampionStats(championStats, previousChampionStats);

            var buildStats = BuildAnalyzer.GetNewBuilds(championStats, previousChampionStats);
            // var buildsDataset = BuildAnalyzer.CreateBuildDataset(buildStats);

            var dataset = new Dataset
            {
                Items = itemStats,
                Runes = runeStats,
                Champions = championStats,
                Version = patch,
                Builds = buildStats,
                ChampionMatches = championStats.Sum(s => s.Matches),
                PreviousChampionMatches = previousChampionStats.Sum(s => s.Matches),
            };

            return JsonSerializer.Serialize(dataset, _jsonSerializerOptions);
        }

        private async Task<string> GetGithubDatasetSha()
        {
            var contents = await _client.Repository.Content.GetAllContents(_owner, _repository, _path);

            var content = contents.Single(c => c.Name == _fileName);

            _logger.LogInformation("Got Dataset Github SHA {Sha}", content.Sha);

            return content.Sha;
        }
    }
}