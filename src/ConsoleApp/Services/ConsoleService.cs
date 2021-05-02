using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Champions.Commands;
using LeagueOfItems.Application.Champions.Queries;
using LeagueOfItems.Application.Github.Commands;
using LeagueOfItems.Application.Items.Commands;
using LeagueOfItems.Application.Items.Queries;
using LeagueOfItems.Application.Riot.Queries;
using LeagueOfItems.Application.Runes.Commands;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.ConsoleApp.Services
{
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

                switch (args[1])
                {
                    case "riot":
                        var version = await _mediator.Send(new GetRiotVersionQuery(), cancellationToken);

                        await _mediator.Send(new GetRiotChampionDataCommand(version), cancellationToken);
                        await _mediator.Send(new GetRiotItemDataCommand(version), cancellationToken);
                        await _mediator.Send(new GetRiotRuneDataCommand(version), cancellationToken);

                        break;
                    case "ugg":
                        await _mediator.Send(new GetUggChampionDataCommand(), cancellationToken);
                        await _mediator.Send(new GetUggItemDataCommand(), cancellationToken);
                        await _mediator.Send(new GetUggRuneDataCommand(), cancellationToken);

                        break;
                    case "github":
                        await _mediator.Send(new UploadGithubCommand(), cancellationToken);

                        break;
                    case "empty-db":
                        await _mediator.Send(new DeleteAllItemsCommand(), cancellationToken);
                        await _mediator.Send(new DeleteAllRunesCommand(), cancellationToken);
                        await _mediator.Send(new DeleteAllChampionsCommand(), cancellationToken);

                        await _mediator.Send(new DeleteAllItemDataCommand(), cancellationToken);
                        await _mediator.Send(new DeleteAllRuneDataCommand(), cancellationToken);
                        break;
                    case "champion":
                        var champion =
                            await _mediator.Send(new GetChampionQuery(int.Parse(args[2])), cancellationToken);

                        var championJson = JsonSerializer.Serialize(champion, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                        Console.WriteLine(championJson);
                        break;
                    case "item":
                        var item = await _mediator.Send(new GetItemQuery(int.Parse(args[2])), cancellationToken);

                        var itemJson = JsonSerializer.Serialize(item, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                        Console.WriteLine(itemJson);
                        break;
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
}