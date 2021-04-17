using System;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.ConsoleApp.Services
{
    public class ConsoleService : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IGithubService _githubService;
        private readonly ILogger<ConsoleService> _logger;
        private readonly IRiotDataService _riotDataService;
        private readonly IUggDataService _uggDataService;

        public ConsoleService(IRiotDataService riotDataService, IUggDataService uggDataService,
            ILogger<ConsoleService> logger, IHostApplicationLifetime appLifetime,
            IGithubService githubService)
        {
            _riotDataService = riotDataService;
            _uggDataService = uggDataService;
            _logger = logger;
            _appLifetime = appLifetime;
            _githubService = githubService;
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
                        await _riotDataService.SaveAll();
                        break;
                    case "ugg":
                        await _uggDataService.SaveForAllChampions();
                        break;
                    case "github":
                        await _githubService.StoreDataset();
                        break;
                    case "empty-db":
                        await _riotDataService.DeleteChampions();
                        await _riotDataService.DeleteRunes();
                        await _riotDataService.DeleteItems();

                        await _uggDataService.DeleteItemData();
                        await _uggDataService.DeleteRuneData();
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