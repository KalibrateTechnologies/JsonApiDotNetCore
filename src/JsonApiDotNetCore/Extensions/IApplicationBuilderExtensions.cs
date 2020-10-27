using JsonApiDotNetCore.Builders;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Internal;
using JsonApiDotNetCore.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace JsonApiDotNetCore.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds necessary components such as routing to your application
        /// </summary>
        /// <param name="app"></param>
        /// <param name="useMvc"></param>
        /// <returns></returns>
        public static void UseJsonApi(this IApplicationBuilder app)
        {
            DisableDetailedErrorsIfProduction(app);
            LogContextGraphValidations(app);

            // An endpoint is selected and set on the HttpContext if a match is found
            app.UseRouting();

            // middleware to run after routing occurs.
            app.UseMiddleware<RequestMiddleware>();

            // Executes the endpoitns that was selected by routing.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void DisableDetailedErrorsIfProduction(IApplicationBuilder app)
        {
            var webHostEnvironment = (IWebHostEnvironment) app.ApplicationServices.GetService(typeof(IWebHostEnvironment));
            if (webHostEnvironment.EnvironmentName == "Production")
            {
                JsonApiOptions.DisableErrorStackTraces = true;
                JsonApiOptions.DisableErrorSource = true;
            }
        }

        private static void LogContextGraphValidations(IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetService(typeof(ILogger<ContextGraphBuilder>)) as ILogger;
            var contextGraph = app.ApplicationServices.GetService(typeof(IContextGraph)) as ContextGraph;

            if (logger != null && contextGraph != null)
            {
                contextGraph.ValidationResults.ForEach((v) =>
                    logger.Log(
                        v.LogLevel,
                        new EventId(),
                        v.Message,
                        exception: null,
                        formatter: (m, e) => m));
            }
        }
    }
}
