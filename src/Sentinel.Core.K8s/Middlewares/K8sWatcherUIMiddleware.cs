using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Sentinel.Core.K8s.Middlewares
{
    public class K8sWatcherUIMiddleware
    {
        private const string EmbeddedFileNamespace = "Sentinel.Core.K8s.k8swatcher_ui_dist";
        // private readonly K8sWatcherMonitor _k8sWatcherMonitor;
        private readonly K8sWatcherUIOptions _options;
        private readonly StaticFileMiddleware _staticFileMiddleware;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public K8sWatcherUIMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            //  K8sWatcherMonitor k8sWatcherMonitor,
            K8sWatcherUIOptions? options
            )
        {
            //   _k8sWatcherMonitor = k8sWatcherMonitor;
            _options = options ?? new K8sWatcherUIOptions();
            _staticFileMiddleware = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory, _options);

            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            _jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
        }


        public async Task Invoke(HttpContext httpContext)
        {
            var httpMethod = httpContext.Request.Method;
            var path = httpContext.Request.Path.Value;

            // If the RoutePrefix is requested (with or without trailing slash), redirect to index URL
            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RoutePrefix)}/?$", RegexOptions.IgnoreCase))
            {
                // Use relative redirect to support proxy environments
                var relativeIndexUrl = string.IsNullOrEmpty(path) || path.EndsWith("/")
                    ? "index.html"
                    : $"{path.Split('/').Last()}/index.html";

                RespondWithRedirect(httpContext.Response, relativeIndexUrl);
                return;
            }

            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/?index.html$", RegexOptions.IgnoreCase))
            {
                await RespondWithIndexHtml(httpContext.Response);
                return;
            }

            await _staticFileMiddleware.Invoke(httpContext);
        }

        private void RespondWithRedirect(HttpResponse response, string location)
        {
            response.StatusCode = 301;
            response.Headers["Location"] = location;
        }

        private async Task RespondWithIndexHtml(HttpResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/html;charset=utf-8";

            using (var stream = _options.IndexStream())
            {
                using var reader = new StreamReader(stream);

                // Inject arguments before writing to response
                var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());
                foreach (var entry in GetIndexArguments())
                {
                    htmlBuilder.Replace(entry.Key, entry.Value);
                }

                await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
            }
        }

        // public async Task Invoke(HttpContext httpContext)
        // {
        //     var httpMethod = httpContext.Request.Method;
        //     var path = httpContext.Request.Path.Value;
        //     if (path == null)
        //     {
        //         await _staticFileMiddleware.Invoke(httpContext);
        //         return;
        //     }

        //     // If the RoutePrefix is requested (with or without trailing slash), redirect to index URL
        //     if (httpMethod == "GET" && Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RoutePrefix)}/?$", RegexOptions.IgnoreCase))
        //     {
        //         // // Use relative redirect to support proxy environments
        //         // var relativeIndexUrl = string.IsNullOrEmpty(path) || path.EndsWith("/")
        //         //     ? "index.html"
        //         //     : $"{path.Split('/').Last()}/index.html";

        //         // RespondWithRedirect(httpContext.Response, relativeIndexUrl);
        //         StringBuilder listbuild = new StringBuilder();
        //         foreach (K8sWatcherMonitorItem item in K8sWatcherMonitor.Attributes.Values)
        //         {
        //             listbuild.AppendLine(string.Format("<ul> "));
        //             listbuild.AppendLine(string.Format(" <li> <b> {0} </b></li>", item.Attribute.Name));
        //             listbuild.AppendLine(string.Format(" <li> Namespace          : {0} </li>", item.Attribute.Namespace));
        //             listbuild.AppendLine(string.Format(" <li> Enabled            : {0} </li>", item.Attribute.Enabled));
        //             listbuild.AppendLine(string.Format(" <li> Description        : {0} </li>", item.Attribute.Description));
        //             listbuild.AppendLine(string.Format(" <li> WatcherHttpTimeout : {0} </li>", item.Attribute.WatcherHttpTimeout));

        //             if (item.HealthCheck is not null)
        //             {

        //                 listbuild.AppendLine(string.Format(" <li> Health Status : {0} </li>", item.HealthCheck.status.ToString()));
        //                 listbuild.AppendLine(string.Format(" <li> Health count : {0} </li>", item.HealthCheck.count.ToString()));
        //                 listbuild.AppendLine(string.Format(" <li> Health LastProcess : {0} </li>", item.HealthCheck.LastProcessUtc.ToLocalTime().ToString()));
        //                 listbuild.AppendLine(string.Format(" <li> Health message : {0} </li>", item.HealthCheck.message.ToString()));


        //             }
        //             listbuild.AppendLine(string.Format(" </ul>"));
        //         }

        //         var html = string.Format("<html><body><h1>{0}</h1><p> {1} </P></body></html>", _options.DocumentTitle, listbuild.ToString());

        //         await httpContext.Response.WriteAsync(html);
        //         return;
        //     }

        //     // if (httpMethod == "GET" && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/?index.html$", RegexOptions.IgnoreCase))
        //     // {
        //     //     // await RespondWithIndexHtml(httpContext.Response);
        //     //     await httpContext.Response.WriteAsync("<html><body><h1>Welcome to My Page!</h1></body></html>");

        //     //     return;
        //     // }

        //     await _staticFileMiddleware.Invoke(httpContext);
        // }


        private string ListK8sWatcherMonitorItems()
        {

            StringBuilder listbuild = new StringBuilder();
            foreach (K8sWatcherMonitorItem item in K8sWatcherMonitor.Attributes.Values)
            {

                // <div class="g-col-6 g-col-md-4">
                //     <div class="card">
                //         <div class="card-header">
                //             Featured
                //         </div>
                //         <ul class="list-group list-group-flush">
                //             <li class="list-group-item">An item</li>
                //             <li class="list-group-item">A second item</li>
                //             <li class="list-group-item">A third item</li>
                //         </ul>
                //     </div>
                // </div>


                listbuild.AppendLine(string.Format("<div class=\"g-col-6 g-col-md-4\">"));
                listbuild.AppendLine(string.Format("<div class=\"card\">"));
                listbuild.AppendLine(string.Format("<div class=\"card-header\">"));
                listbuild.AppendLine(string.Format("<b> {0} </b>", item.Attribute.Name));
                listbuild.AppendLine(string.Format("</div>"));

                listbuild.AppendLine(string.Format("<ul class=\"list-group list-group-flush\" >"));

                listbuild.AppendLine(string.Format(" <li class=\"list-group-item\"> <b> Namespace </b>         : {0} </li>", item.Attribute.Namespace));
                listbuild.AppendLine(string.Format(" <li class=\"list-group-item\"> <b> Enabled </b>            : {0} </li>", item.Attribute.Enabled));
                listbuild.AppendLine(string.Format(" <li class=\"list-group-item\"> <b> Description </b>       : {0} </li>", item.Attribute.Description));
                listbuild.AppendLine(string.Format(" <li class=\"list-group-item\"> <b> WatcherHttpTimeout </b> : {0} </li>", item.Attribute.WatcherHttpTimeout));

                if (item.HealthCheck is not null)
                {

                    listbuild.AppendLine(string.Format(" <li class=\"list-group-item\"> <b> Health Status </b> : {0} </li>", item.HealthCheck.status.ToString()));
                    listbuild.AppendLine(string.Format(" <li class=\"list-group-item\"> <b> Health count </b> : {0} </li>", item.HealthCheck.count.ToString()));
                    listbuild.AppendLine(string.Format(" <li class=\"list-group-item\"> <b> Health LastProcess  </b>: {0} </li>", item.HealthCheck.LastProcessUtc.ToLocalTime().ToString()));
                    listbuild.AppendLine(string.Format(" <li class=\"list-group-item\"> <b> Health message </b> : {0} </li>", item.HealthCheck.message.ToString()));
                }
                listbuild.AppendLine(string.Format(" </ul>"));

                listbuild.AppendLine(string.Format(" </div>"));
                listbuild.AppendLine(string.Format(" </div>"));

            }
            return listbuild.ToString();
        }

        private StaticFileMiddleware CreateStaticFileMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            K8sWatcherUIOptions options)
        {
            var staticFileOptions = new StaticFileOptions
            {
                RequestPath = string.IsNullOrEmpty(options.RoutePrefix) ? string.Empty : $"/{options.RoutePrefix}",
                FileProvider = new EmbeddedFileProvider(typeof(K8sWatcherUIMiddleware).GetTypeInfo().Assembly, EmbeddedFileNamespace),
            };

            return new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
        }


        //private async Task RespondWithIndexHtml(HttpResponse response)
        //{
        //    response.StatusCode = 200;
        //    response.ContentType = "text/html;charset=utf-8";

        //    // using (var stream = _options.IndexStream())
        //    // {
        //    //     using var reader = new StreamReader(stream);

        //    //     // Inject arguments before writing to response
        //    //     var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());
        //    //     foreach (var entry in GetIndexArguments())
        //    //     {
        //    //         htmlBuilder.Replace(entry.Key, entry.Value);
        //    //     }

        //    //     await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
        //    // }
        //}

        private IDictionary<string, string> GetIndexArguments()
        {
            return new Dictionary<string, string>()
            {
                { "%(DocumentTitle)", _options.DocumentTitle },
                { "%(ListK8sWatcherMonitorItems)", ListK8sWatcherMonitorItems() }
                // { "%(HeadContent)", _options.HeadContent },
                // { "%(ConfigObject)", JsonSerializer.Serialize(_options.ConfigObject, _jsonSerializerOptions) },
                // { "%(OAuthConfigObject)", JsonSerializer.Serialize(_options.OAuthConfigObject, _jsonSerializerOptions) },
                // { "%(Interceptors)", JsonSerializer.Serialize(_options.Interceptors) },
            };
        }
    }
}