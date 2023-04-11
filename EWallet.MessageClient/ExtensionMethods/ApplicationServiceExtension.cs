using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.Data;
using EWallet.DataLayer.Implementations;
using EWallet.MessageClient.AsyncDataService;
using EWallet.MessageClient.EventProcessing;
using EWallet.Utility.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EWallet.MessageClient.ExtensionMethods
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration _config)
        {
            services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(_config.GetConnectionString("DefaultConnection")));

            services.Configure<RabbitMQSettings>(_config.GetSection("RabbitMQSettings"));

            services.AddHostedService<MessageBusSubscriber>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IPhotoRepository, PhotoRepository>();

            services.AddScoped<IEventProcessor, EventProcessor>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}
