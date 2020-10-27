using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JsonApiDotNetCoreExample.Data;
using Microsoft.EntityFrameworkCore;
using JsonApiDotNetCore.Extensions;
using JsonApiDotNetCoreExample;

namespace JsonApiDotNetCoreExampleTests.Startups
{
    public class ClientGeneratedIdsStartup : Startup
    {
        public ClientGeneratedIdsStartup(IWebHostEnvironment env)
        : base (env)
        {  }

        public override void ConfigureServices(IServiceCollection services)
        {
            var loggerFactory = new LoggerFactory();

            services.AddLogging(b =>
            {
                b.AddConsole();
            });

            services.AddSingleton<ILoggerFactory>(loggerFactory);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(GetDbConnectionString());
            }, ServiceLifetime.Transient);

            services.AddJsonApi<AppDbContext>(opt =>
            {
                opt.Namespace = "api/v1";
                opt.DefaultPageSize = 5;
                opt.IncludeTotalRecordCount = true;
                opt.AllowClientGeneratedIds = true;
            });
        }
    }
}
