using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Product.API.Infrastructure.Data;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Product.API.Infrastructure.Extensions
{
    public static class ServiceCollecationExtensions
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConnectionSettings>(configuration.GetSection("ConnectionStrings"));
            var connectionOptions = services.BuildServiceProvider().GetRequiredService<IOptions<ConnectionSettings>>();
            services.RegisterDatabaseType(connectionOptions);

            var databaseTypeInstance = services.BuildServiceProvider().GetRequiredService<IDatabaseType>();
            databaseTypeInstance.EnableDatabase(services);

            services.AddDbContext<ProductContext>(options => databaseTypeInstance.GetContextBuilder(options, connectionOptions.Value.DefaultConnection))
                    .AddUnitOfWork<ProductContext>();

            return services;
        }

        public static IServiceCollection RegisterDatabaseType(this IServiceCollection services,
            IOptions<ConnectionSettings> connectionOptions)
        {
            var databaseInterfaceType = typeof(IDatabaseType);
            var instanceType = connectionOptions.Value.DatabaseType.ToString();
            var instance = databaseInterfaceType.Assembly.GetTypes().FirstOrDefault(x =>
                databaseInterfaceType.IsAssignableFrom(x)
                &&
                string.Equals(instanceType, x.Name, StringComparison.OrdinalIgnoreCase));
            services.AddSingleton((IDatabaseType)Activator.CreateInstance(instance));
            return services;
        }

        public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddControllersAsServices();

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(2, 0);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed(host => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Info()
                {
                    Title = "Products API - HTTP API",
                    Version = "v1",
                    Description = "The Products HTTP API."
                });

                options.AddSecurityDefinition("Bearer",
                    new ApiKeyScheme
                    {
                        In = "header",
                        Description = "Please enter into field the word 'Bearer' following by space and JWT",
                        Name = "Authorization",
                        Type = "apiKey",
                    });
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", Enumerable.Empty<string>() }
                });
            });

            return services;
        }

        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
            services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
            return services;
        }

        public static IServiceCollection AddCustomAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingSection);

            var appSettings = appSettingSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            return services;
        }
    }
}
