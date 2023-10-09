using Newtonsoft.Json;

namespace Sentinel.Core.K8s.Models.Istio
{
    public class HttpRewrite
    {

        /// <summary>
        /// Gets/sets the value used to rewrite the path (or the prefix) portion of the URI. If the original URI was matched based on prefix, the value provided in this field will replace the corresponding matched prefix.
        /// </summary>
        [JsonProperty(PropertyName = "uri")]
        public string? Uri { get; set; }

        /// <summary>
        /// Gets/sets the value used to rewrite the Authority/Host header.
        /// </summary>
        [JsonProperty(PropertyName = "authority")]
        public string? Authority { get; set; }

    }
}