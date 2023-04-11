using EWallet.Utility.JwtHandler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace EWallet.API.ExtensionMethods
{
    public static class JwtAuthenticationExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration _config)
        {
            services.AddScoped<IJwtTokenMethod, JwtTokenMethod>();

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(jwt =>
                {
                    var key = Encoding.ASCII.GetBytes(_config["JwtSettings:Secret"]);

                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false, //ToDO Update later
                        ValidateAudience = false, //ToDO Update later
                        RequireExpirationTime = false, //ToDO Update later
                        ValidateLifetime = true,
                    };
                });

            services.Configure<DataProtectionTokenProviderOptions>(option => option.TokenLifespan = TimeSpan.FromMinutes(30));

            return services;
        }
    }
}
