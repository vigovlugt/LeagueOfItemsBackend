using System.Threading.Tasks;
using LeagueOfItems.Models;
using LeagueOfItems.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UggController : ControllerBase
    {
        private readonly ILogger<UggController> _logger;
        private ItemContext _context;
        private readonly IUggDataService _uggService;

        public UggController(ILogger<UggController> logger, ItemContext context, IUggDataService uggService)
        {
            _logger = logger;
            _context = context;
            _uggService = uggService;
        }
        
        [HttpGet]
        public async Task Get()
        {
            await _uggService.SaveForAllChampions();
        }
    }
}