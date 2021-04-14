using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LeagueOfItems.Models.Filters;

namespace LeagueOfItems.Services
{
    public interface IExportService
    {
        Task ExportAll();
        Task ExportItems();
        Task ExportRunes();
    }

    public class ExportService : IExportService
    {
        private readonly IRuneService _runeService;
        private readonly IItemService _itemService;
        
        public ExportService(IRuneService runeService, IItemService itemService)
        {
            _runeService = runeService;
            _itemService = itemService;
        }

        public async Task ExportAll()
        {
            await Task.WhenAll(ExportItems(), ExportRunes());
        }

        public async Task ExportItems()
        {
            var itemStats = await _itemService.GetAllItems(new ItemFilter());

            await Task.WhenAll(itemStats.Select(async item =>
            {
                var json = JsonSerializer.Serialize(item);

                await File.WriteAllTextAsync(
                    Path.Combine(Environment.CurrentDirectory, "Data", "Items", $"{item.Id}.json"), json);
            }));
        }

        public async Task ExportRunes()
        {
            var runeStats = await _runeService.GetAllRunes(new ItemFilter());

            await Task.WhenAll(runeStats.Select(async rune =>
            {
                var json = JsonSerializer.Serialize(rune);

                await File.WriteAllTextAsync(
                    Path.Combine(Environment.CurrentDirectory, "Data", "Runes", $"{rune.Id}.json"), json);
            }));
        }
    }
}