using Microsoft.Extensions.DependencyInjection;
using LeagueOfItems.Models;
using LeagueOfItems.Services;

namespace LeagueOfItems
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ItemContext>();
            services.AddHttpClient();
            services.AddAutoMapper(typeof(Startup));

            services.AddScoped<IRiotDataService, RiotDataService>();
            services.AddScoped<IUggDataService, UggDataService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IRuneService, RuneService>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<IGithubService, GithubService>();
        }
    }
}