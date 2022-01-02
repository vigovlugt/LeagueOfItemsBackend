using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace LeagueOfItems.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

        return services;
    }
}