using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JsonApiDotNetCoreExample.Data;
using Microsoft.EntityFrameworkCore;
using JsonApiDotNetCore.Extensions;
using JsonApiDotNetCoreExample.Models;
using JsonApiDotNetCoreExample.Resources;
using JsonApiDotNetCore.Models;

namespace JsonApiDotNetCoreExample
{
    public class Startup
    {
        public readonly IConfiguration Config;

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Config = builder.Build();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(b =>
            {
                b.SetMinimumLevel(LogLevel.Warning);
                b.AddConsole();
                b.AddConfiguration(Config.GetSection("Logging"));
            });

            services.AddDbContext<AppDbContext>(options => 
                options.UseNpgsql(GetDbConnectionString()), ServiceLifetime.Transient)
                .AddJsonApi(options => {
                    options.Namespace = "api/v1";
                    options.DefaultPageSize = 5;
                    options.IncludeTotalRecordCount = true;
                    options.BuildContextGraph(builder =>
                    {
                        builder.AddResource<User>("user");
                    });
                },
                services.AddMvcCore().AddNewtonsoftJson());

            services.AddScoped<ResourceDefinition<User>, UserResource>();
        }

        public virtual void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory,
            AppDbContext context)
        {
            context.Database.EnsureCreated();;

            app.UseJsonApi();
        }

        public string GetDbConnectionString() => Config["Data:DefaultConnection"];
    }
}
