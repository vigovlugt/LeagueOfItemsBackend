using System;
using System.Threading.Tasks;
using LeagueOfItems.Application;
using LeagueOfItems.ConsoleApp.Services;
using LeagueOfItems.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LeagueOfItems.ConsoleApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).RunConsoleAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddHostedService<ConsoleService>();
                services.AddApplication();
                services.AddInfrastructure();
            })
            .ConfigureAppConfiguration(config =>
                config
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                        true)
                    .AddUserSecrets<Program>(true)
                    .AddEnvironmentVariables())
            .UseSerilog((hostContext, loggerConfiguration) =>
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration));
    }
}