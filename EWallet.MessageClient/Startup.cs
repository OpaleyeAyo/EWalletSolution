using EWallet.MessageClient.ExtensionMethods;
using EWallet.Utility.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace EWallet.MessageClient
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
            services.Configure<SettlementAccountNumbers>(_config.GetSection("SettlementAccountNumbers"));

            services.Configure<CloudinarySettings>(_config.GetSection("CloudinarySettings"));

            services.Configure<SendGridSettings>(_config.GetSection("SendGridSettings"));

            services.Configure<RabbitMQSettings>(_config.GetSection("RabbitMQSettings"));

            var sendGridSender = _config["SendGridSettings:Sender"];
            var from = _config["SendGridSettings:From"];
            var sendGridKey = _config["SendGridSettings:SendGridKey"];

            services
                .AddFluentEmail(sendGridSender, from)
                .AddRazorRenderer()
                .AddSendGridSender(sendGridKey);

            services.AddApplicationServices(_config);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EWallet.MessageClient", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EWallet.MessageClient v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
