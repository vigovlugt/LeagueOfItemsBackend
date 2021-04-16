using LeagueOfItems.Application.Contexts;
using LeagueOfItems.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LeagueOfItems.ConsoleApp
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
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