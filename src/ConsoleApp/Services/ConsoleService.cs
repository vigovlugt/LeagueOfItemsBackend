using System;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.BuildPaths.Commands;
using LeagueOfItems.Application.Champions.Commands;
using LeagueOfItems.Application.Datasets.Query;
using LeagueOfItems.Application.Github.Commands;
using LeagueOfItems.Application.Items.Commands;
using LeagueOfItems.Application.Patches.Queries;
using LeagueOfItems.Application.Riot.Queries;
using LeagueOfItems.Application.Runes.Commands;
using LeagueOfItems.Application.Ugg.Helpers;
using LeagueOfItems.Application.Ugg.Queries;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.ConsoleApp.Services;

public class ConsoleService : IHostedService
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ILogger<ConsoleService> _logger;
    private readonly IMediator _mediator;

    public ConsoleService(ILogger<ConsoleService> logger, IHostApplicationLifetime appLifetime, IMediator mediator)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _mediator = mediator;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(async () =>
        {
            var args = Environment.GetCommandLineArgs();

            if (args.Length <= 1)
            {
                _logger.LogCritical("Usage: dotnet run ugg/riot/export");
                _appLifetime.StopApplication();
                return;
            }

            for (var i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "riot":
                        var versions = await _mediator.Send(new GetRiotVersionsQuery(), cancellationToken);
                        var version = versions[0];
                        var previousRiotVersion = LolVersionHelper.GetPreviousVersion(versions);

                        await _mediator.Send(new DeleteAllChampionsCommand(), cancellationToken);
                        await _mediator.Send(new DeleteAllItemsCommand(), cancellationToken);
                        await _mediator.Send(new DeleteAllRunesCommand(), cancellationToken);

                        await _mediator.Send(new GetRiotChampionDataCommand(version), cancellationToken);
                        await _mediator.Send(new GetRiotItemDataCommand(version), cancellationToken);
                        await _mediator.Send(new GetRiotRuneDataCommand(version), cancellationToken);
                            
                        await _mediator.Send(new GetRiotChampionDataCommand(previousRiotVersion),
                            cancellationToken);
                        await _mediator.Send(new GetRiotItemDataCommand(previousRiotVersion), cancellationToken);
                        await _mediator.Send(new GetRiotRuneDataCommand(previousRiotVersion), cancellationToken);

                        break;
                    case "ugg":
                        await _mediator.Send(new DeleteAllRuneDataCommand());
                        await _mediator.Send(new DeleteAllItemDataCommand());
                        await _mediator.Send(new DeleteAllChampionDataCommand());
                        await _mediator.Send(new DeleteAllBuildPathDataCommand());

                        var uggVersion = await _mediator.Send(new GetUggVersionQuery(), cancellationToken);
                        if (string.IsNullOrEmpty(uggVersion))
                        {
                            _logger.LogCritical("UGG version not found");
                            return;
                        }

                        await _mediator.Send(new GetUggChampionDataCommand(uggVersion), cancellationToken);
                        await _mediator.Send(new GetUggItemDataCommand(uggVersion), cancellationToken);
                        await _mediator.Send(new GetUggRuneDataCommand(uggVersion), cancellationToken);
                        await _mediator.Send(new GetUggBuildPathDataCommand(uggVersion), cancellationToken);

                        _logger.LogInformation("Downloading info for previous UGG Patch");

                        var previousVersion = LolVersionHelper.GetPreviousVersion(uggVersion);
                        await _mediator.Send(new GetUggChampionDataCommand(previousVersion), cancellationToken);
                        await _mediator.Send(new GetUggItemDataCommand(previousVersion), cancellationToken);
                        await _mediator.Send(new GetUggRuneDataCommand(previousVersion), cancellationToken);
                        await _mediator.Send(new GetUggBuildPathDataCommand(previousVersion), cancellationToken);

                        break;
                    case "github":
                        var dataset = await _mediator.Send(new GetDatasetCommand(), cancellationToken);
                        await _mediator.Send(new UploadGithubCommand(dataset), cancellationToken);

                        break;
                    case "run":

                        break;
                    case "empty-db":
                        await _mediator.Send(new DeleteAllItemsCommand(), cancellationToken);
                        await _mediator.Send(new DeleteAllRunesCommand(), cancellationToken);
                        await _mediator.Send(new DeleteAllChampionsCommand(), cancellationToken);

                        await _mediator.Send(new DeleteAllItemDataCommand(), cancellationToken);
                        await _mediator.Send(new DeleteAllRuneDataCommand(), cancellationToken);
                        break;
                }
            }

            _appLifetime.StopApplication();
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}