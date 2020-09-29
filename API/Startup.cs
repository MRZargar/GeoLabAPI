using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GeoLabAPI
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
            services.AddControllers();

            services.AddDbContext<geolabContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
                    .ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>());
            
            services.AddDbContext<geolabContext>(options =>
                options.UseNpgsql(string.Format("Server={};Port={};DataBase={};User Id={};Password={};",
                        Environment.GetEnvironmentVariable("DB_HOST"),
                        Environment.GetEnvironmentVariable("DB_PORT"),
                        Environment.GetEnvironmentVariable("DB_NAME"),
                        Environment.GetEnvironmentVariable("DB_USER"),
                        Environment.GetEnvironmentVariable("DB_PASS"))
                    ).ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
