using EWallet.DataLayer.AutoMapper;
using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.Data;
using EWallet.DataLayer.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using EWallet.Utility.HttpContex;
using EWallet.API.AsyncDataTransfer;
using Hangfire;
using EWallet.API.BgImplementation;
using DinkToPdf.Contracts;
using DinkToPdf;
using EWallet.API.Filters;

namespace EWallet.API.ExtensionMethods
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration _config)
        {
            services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(_config.GetConnectionString("DefaultConnection")));

            services.AddHangfire(config => config
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(_config.GetConnectionString("DefaultConnection"))
            );

            //services.AddMvc(options => options.Filters.Add<ValidationFilter>());

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddHangfireServer();

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddScoped<ModelValidationAttributeFilter>();

            services.AddScoped<ICurrencyRepository, CurrencyRepository>();

            services.AddTransient<IBackgroundServiceRepository, BgServices>();

            services.AddScoped<ISettlementAccountRepository, SettlementAccountRepository>();

            services.AddScoped<IWalletRepository, WalletRepository>();

            services.AddScoped<IPhotoRepository, PhotoRepository>();

            services.AddScoped<ITransactionRepository, TransactionRepository>();

            services.AddScoped<IUserContext, UserContext>();

            services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

            //services.AddSingleton<IMessageBusClient, MessageBusClient>();
            services.AddScoped<IMessageBusClient, MessageBusClient>();

            services.AddIdentity<IdentityUser, IdentityRole>(option =>
            {
                option.SignIn.RequireConfirmedAccount = true;
                //option.SignIn.RequireConfirmedEmail = true;

            })
                .AddEntityFrameworkStores<AppDbContext>();

            return services;
        }
    }
}
