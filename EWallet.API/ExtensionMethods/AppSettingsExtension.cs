using EWallet.Utility.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EWallet.API.ExtensionMethods
{
    public static class AppSettingsExtension
    {
        public static IServiceCollection AddAppSetttings(this IServiceCollection services, IConfiguration _config)
        {
            services.Configure<JwtSettings>(_config.GetSection("JwtSettings"));

            services.Configure<SettlementAccountNumbers>(_config.GetSection("SettlementAccountNumbers"));

            services.Configure<BgServiceSettings>(_config.GetSection("BgServiceSettings"));

            services.Configure<CloudinarySettings>(_config.GetSection("CloudinarySettings"));

            services.Configure<SendGridSettings>(_config.GetSection("SendGridSettings"));

            services.Configure<RabbitMQSettings>(_config.GetSection("RabbitMQSettings"));

            return services;
        }
    }
}
