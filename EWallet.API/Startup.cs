using EWallet.API.ExtensionMethods;
using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.Data;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace EWallet.API
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAppSetttings(_config);

            services.AddApplicationServices(_config);

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EWallet.API", Version = "v1" });
            });

            services.ConfigureCors();

            services.AddSwaggerConfiguration();

            //services.AddModelValidation();

            services.AddAPIVersioning();

            services.AddJwtAuthentication(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EWallet.API v1"));
            }
            
            SeedCurrencyDb.SeedCurrency(app);

            SeedSettlementAccountDb.SeedSettlementDb(app);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate<IBackgroundServiceRepository>(x => x.AddInterest(), "0 0 0 * * ?");

            RecurringJob.AddOrUpdate<IBackgroundServiceRepository>(x => x.CheckAccounts(), "0 */5 * ? * *");
        }
    }
}
