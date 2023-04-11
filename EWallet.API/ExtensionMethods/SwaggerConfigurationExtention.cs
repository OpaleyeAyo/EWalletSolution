using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace EWallet.API.ExtensionMethods
{
    public static class SwaggerConfigurationExtention
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(option =>
            {
                option.ResolveConflictingActions(a => a.First());
                option.OperationFilter<RemoveVersionFromParameter>();
                option.DocumentFilter<ReplaceVersionWithExactValueInPath>();

                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }
                    }

                });
            });

            return services;
        }

        private class RemoveVersionFromParameter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var versionParameter = operation.Parameters.Single(x => x.Name == "version");

                operation.Parameters.Remove(versionParameter);
            }
        }

        private class ReplaceVersionWithExactValueInPath : IDocumentFilter
        {
            public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            {
                var paths = swaggerDoc.Paths;
                swaggerDoc.Paths = new OpenApiPaths();

                foreach (var path in paths)
                {
                    var key = path.Key.Replace("v{version}", swaggerDoc.Info.Version);

                    var value = path.Value;

                    swaggerDoc.Paths.Add(key, value);
                }
            }
        }
    }
}
