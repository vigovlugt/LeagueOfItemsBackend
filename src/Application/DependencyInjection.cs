using LeagueOfItems.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LeagueOfItems.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IRiotDataService, RiotDataService>();
            services.AddScoped<IUggDataService, UggDataService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IRuneService, RuneService>();
            services.AddScoped<IGithubService, GithubService>();

            services.AddHttpClient();

            return services;
        }
    }
}