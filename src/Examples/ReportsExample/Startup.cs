using JsonApiDotNetCore.Extensions;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ReportsExample
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
            var mvcBuilder = services.AddMvcCore();
            services.AddJsonApi(opt =>
            {
                opt.BuildContextGraph(builder =>
                {
                    builder.AddResource<Report>("reports");
                });
                opt.Namespace = "api";
            }, mvcBuilder);

            services.AddScoped<IGetAllService<Report>, ReportService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            app.UseJsonApi();
        }
    }
}
