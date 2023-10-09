using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Sentinel.Core.K8s.Middlewares
{
    public static class K8sWatcherUIBuilderExtensions
    {
        /// <summary>
        /// Register the K8sWatcherUI middleware with provided options
        /// </summary>
        public static IApplicationBuilder UseK8sWatcherUI(this IApplicationBuilder app, K8sWatcherUIOptions options)
        {
            return app.UseMiddleware<K8sWatcherUIMiddleware>(options);
        }

        /// <summary>
        /// Register the K8sWatcherUI middleware with optional setup action for DI-injected options
        /// </summary>
        public static IApplicationBuilder UseK8sWatcherUI(
            this IApplicationBuilder app,
            Action<K8sWatcherUIOptions>? setupAction = null)
        {
            K8sWatcherUIOptions options;
            using (var scope = app.ApplicationServices.CreateScope())
            {
                options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<K8sWatcherUIOptions>>().Value;
                setupAction?.Invoke(options);
            }

            // To simplify the common case, use a default that will work with the SwaggerMiddleware defaults
            // if (options.ConfigObject.Urls == null)
            // {
            //     var hostingEnv = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            //     options.ConfigObject.Urls = new[] { new UrlDescriptor { Name = $"{hostingEnv.ApplicationName} v1", Url = "v1/swagger.json" } };
            // }

            return app.UseK8sWatcherUI(options);
        }
    }
}