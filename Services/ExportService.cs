using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueOfItems.Models;
using LeagueOfItems.Models.Filters;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Services
{
    public interface IExportService
    {
        Task ExportAll();
    }

    public class ExportService : IExportService
    {
        private readonly IRuneService _runeService;
        private readonly IItemService _itemService;
        private readonly ILogger<ExportService> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ExportService(IRuneService runeService, IItemService itemService, ILogger<ExportService> logger)
        {
            _runeService = runeService;
            _itemService = itemService;
            _logger = logger;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task ExportAll()
        {
            _logger.LogInformation("Exporting all runes and items");

            var itemStats = await _itemService.GetAllItems(new ItemFilter());
            var runeStats = await _runeService.GetAllRunes(new ItemFilter());

            var dataset = new ItemRuneDataset
            {
                Items = itemStats,
                Runes = runeStats
            };

            var json = JsonSerializer.Serialize(dataset, _jsonSerializerOptions);

            await File.WriteAllTextAsync(
                Path.Combine(Environment.CurrentDirectory, "Data", "dataset.json"), json);
        }
    }
}