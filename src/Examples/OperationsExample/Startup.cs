using JsonApiDotNetCore.Extensions;
using JsonApiDotNetCoreExample.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OperationsExample
{
    public class Startup
    {
        public readonly IConfiguration Config;

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
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

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(GetDbConnectionString()), ServiceLifetime.Scoped);

            services.AddJsonApi<AppDbContext>(opt => opt.EnableOperations = true);
        }

        public virtual void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory,
            AppDbContext context)
        {
            context.Database.EnsureCreated();
            
            app.UseJsonApi();
        }

        public string GetDbConnectionString() => Config["Data:DefaultConnection"];
    }
}
