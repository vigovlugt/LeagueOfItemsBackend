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
    public class RunesController
    {
        private readonly ILogger<RunesController> _logger;
        private readonly IRuneService _service;

        public RunesController(ILogger<RunesController> logger, IRuneService service)
        {
            _logger = logger;
            _service = service;
        }
        
        [HttpGet]
        public async Task<List<RuneStats>> Get([FromQuery] ItemFilter filter)
        {
            return await _service.GetAllRunes(filter);
        }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<RuneStats> Get([FromRoute] int id, [FromQuery] ItemFilter filter)
        {
            return await _service.GetRuneStats(id, filter);
        }
    }
}