using System;
using System.Threading.Tasks;
using LeagueOfItems.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LeagueOfItems
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ConsoleService>();

                    var startup = new Startup();
                    startup.ConfigureServices(services);
                })
                .ConfigureAppConfiguration(config =>
                    config
                        .AddUserSecrets<Program>()
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                            true))
                .UseSerilog((hostContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Seq("http://localhost:5341")
                        .WriteTo.Console();
                });
    }
}