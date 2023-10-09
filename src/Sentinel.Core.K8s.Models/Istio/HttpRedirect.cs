using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sentinel.Core.K8s.Models.Istio
{
    public class HttpRedirect
    {

        /// <summary>
        /// Initializes a new <see cref="HttpRedirect"/>
        /// </summary>
        public HttpRedirect()
        {
            this.RedirectCode = 301;
        }

        /// <summary>
        /// Gets/sets the value used to overwrite the Path portion of the URL. Note that the entire path will be replaced, irrespective of the request URI being matched as an exact path or prefix.
        /// </summary>
        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; } = default!;

        /// <summary>
        /// Gets/sets the value used to overwrite the Authority/Host portion of the URL.
        /// </summary>
        [JsonProperty(PropertyName = "authority")]
        public string Authority { get; set; } = default!;

        /// <summary>
        /// Gets/sets the http status code to use in the redirect response. The default response code is MOVED_PERMANENTLY (301).
        /// </summary>
        [JsonProperty(PropertyName = "redirectCode")]
        public uint RedirectCode { get; set; }

    }
}