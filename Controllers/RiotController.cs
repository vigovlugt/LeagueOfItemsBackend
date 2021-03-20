using System.Linq;
using System.Threading.Tasks;
using LeagueOfItems.Models;
using LeagueOfItems.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiotController : ControllerBase
    {
        private readonly ILogger<RiotController> _logger;
        private ItemContext _context;
        private readonly IRiotDataService _riotService;

        public RiotController(ILogger<RiotController> logger, ItemContext context, IRiotDataService riotService)
        {
            _logger = logger;
            _context = context;
            _riotService = riotService;
        }

        [HttpGet]
        public async Task Get()
        {
            var riotItems = await _riotService.GetItems();

            var items = riotItems.Select(Item.FromRiotItem).ToList();

            await _riotService.SaveItems(items);

            var riotChampions = await _riotService.GetChampions();

            var champions = riotChampions.Select(Champion.FromRiotChampion).ToList();

            await _riotService.SaveChampions(champions);
        }
    }
}