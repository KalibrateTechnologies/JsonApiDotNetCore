using JsonApiDotNetCore.Data;
using JsonApiDotNetCore.Services;
using JsonApiDotNetCoreExample.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OperationsExample;
using UnitTests;

namespace OperationsExampleTests
{
    public class TestStartup : Startup
    {
        public TestStartup(IWebHostEnvironment env) : base(env)
        { }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddScoped<IScopedServiceProvider, TestScopedServiceProvider>();
            services.AddSingleton<IDbContextResolver, DbContextResolver<AppDbContext>>();
        }
    }
}
