using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.API.Infrastructure.Extensions;
using Product.API.Infrastructure.Services;

namespace Product.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomMVC(Configuration)
                    .AddCustomAuth(Configuration)
                    .AddCustomDbContext(Configuration)
                    .AddSwagger(Configuration)
                    .AddScoped<IProductService, ProductService>()
                    .AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API v1");
            });

            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
