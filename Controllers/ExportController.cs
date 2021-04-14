using System.Threading.Tasks;
using LeagueOfItems.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeagueOfItems.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController
    {
        private readonly IExportService _service;

        public ExportController(IExportService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task Get()
        {
            await _service.ExportAll();
        }
    }
}