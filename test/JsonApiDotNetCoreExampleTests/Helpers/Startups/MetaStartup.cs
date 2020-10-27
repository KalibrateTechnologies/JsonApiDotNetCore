using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JsonApiDotNetCoreExample.Data;
using Microsoft.EntityFrameworkCore;
using JsonApiDotNetCore.Extensions;
using JsonApiDotNetCoreExample;
using JsonApiDotNetCore.Services;
using JsonApiDotNetCoreExampleTests.Services;

namespace JsonApiDotNetCoreExampleTests.Startups
{
    public class MetaStartup : Startup
    {
        public MetaStartup(IWebHostEnvironment env)
        : base (env)
        {  }

        public override void ConfigureServices(IServiceCollection services)
        {
            var loggerFactory = new LoggerFactory();
            services.AddLogging(b =>
            {
                b.SetMinimumLevel(LogLevel.Warning);
                b.AddConsole();
            });

            services
                .AddSingleton<ILoggerFactory>(loggerFactory)
                .AddDbContext<AppDbContext>(options => 
                    options.UseNpgsql(GetDbConnectionString()), ServiceLifetime.Transient)
                .AddJsonApi<AppDbContext>(options => {
                    options.Namespace = "api/v1";
                    options.DefaultPageSize = 5;
                    options.IncludeTotalRecordCount = true;
                })
                .AddScoped<IRequestMeta, MetaService>();
        }
    }
}
