using JsonApiDotNetCore.Extensions;
using JsonApiDotNetCore.Services;
using JsonApiDotNetCoreExample.Data;
using JsonApiDotNetCoreExample.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NoEntityFrameworkExample.Services;
using Microsoft.EntityFrameworkCore;

namespace NoEntityFrameworkExample
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(b =>
            {
                b.AddConfiguration(Configuration.GetSection("Logging"));
            });

            // Add framework services.
            var mvcBuilder = services.AddMvcCore();

            services.AddJsonApi(options => {
                options.Namespace = "api/v1";
                options.BuildContextGraph((builder) => {
                    builder.AddResource<TodoItem>("custom-todo-items");
                });
            }, mvcBuilder);

            services.AddScoped<IResourceService<TodoItem>, TodoItemService>();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>(); 
            optionsBuilder.UseNpgsql(Configuration.GetValue<string>("Data:DefaultConnection")); 
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton(optionsBuilder.Options);
            services.AddScoped<AppDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, AppDbContext context)
        {
            context.Database.EnsureCreated();

            app.UseJsonApi();
        }
    }
}
