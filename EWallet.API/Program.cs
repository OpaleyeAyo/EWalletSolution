using EWallet.DataLayer.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;

namespace EWallet.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            try
            {
                Log.Information("Application Starting........");

                var host = CreateHostBuilder(args).Build();//.Run();

                using var scope = host.Services.CreateScope();

                var services = scope.ServiceProvider;

                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await SeedIdentityRoles.SeedRoles(roleManager);

                await SeedIdentityRoles.SeedSuperAdmin(userManager, roleManager);

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Application failed to start.......");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
