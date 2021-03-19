using System.Collections.Generic;
using System.Threading.Tasks;
using LeagueOfItems.Models;
using LeagueOfItems.Services;
using LeagueOfItems.ViewModels;
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
        public async Task<List<ItemViewModel>> Get([FromQuery] ItemFilter filter)
        {
            return await _service.GetAllItems(filter);
        }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<ItemViewModel> Get([FromRoute] int id, [FromQuery] ItemFilter filter)
        {
            return await _service.GetItemStats(id, filter);
        }
    }
    
    
}