using System.Collections.Generic;
using System.Threading.Tasks;
using LeagueOfItems.Models;
using LeagueOfItems.Models.Filters;
using LeagueOfItems.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ILogger<ItemsController> _logger;
        private readonly IItemService _service;

        public ItemsController(ILogger<ItemsController> logger, IItemService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public async Task<List<ItemStats>> Get([FromQuery] ItemFilter filter)
        {
            return await _service.GetAllItems(filter);
        }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<ItemStats> Get([FromRoute] int id, [FromQuery] ItemFilter filter)
        {
            return await _service.GetItemStats(id, filter);
        }
    }
    
    
}