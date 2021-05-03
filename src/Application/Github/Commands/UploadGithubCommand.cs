using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Champions.Queries;
using LeagueOfItems.Application.Items.Queries;
using LeagueOfItems.Application.Runes.Queries;
using LeagueOfItems.Application.Ugg.Queries;
using LeagueOfItems.Domain.Models;
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

            _logger.LogInformation("Getting all Items");
            var itemStats = await _mediator.Send(new GetAllItemsQuery());

            _logger.LogInformation("Getting all Runes");
            var runeStats = await _mediator.Send(new GetAllRunesQuery());

            _logger.LogInformation("Getting all Champions");
            var championStats = await _mediator.Send(new GetAllChampionsQuery());
            var version = await _mediator.Send(new GetUggVersionQuery());

            var dataset = new ItemRuneDataset
            {
                Items = itemStats,
                Runes = runeStats,
                Champions = championStats,
                Version = version
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