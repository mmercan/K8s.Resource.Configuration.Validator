using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sentinel.Core.K8s.Models.Istio
{
    public class HeadersOperations
    {

        /// <summary>
        /// Gets/sets the mapping of the headers to overwrite
        /// </summary>
        [JsonProperty(PropertyName = "set")]
        public IDictionary<string, string> Set { get; set; } = default!;

        /// <summary>
        /// Gets/sets the key/value pairs to append (will create a comma-separated list of values)
        /// </summary>
        [JsonProperty(PropertyName = "add")]
        public IDictionary<string, string> Add { get; set; } = default!;

        /// <summary>
        /// Gets/sets the headers to remove
        /// </summary>
        [JsonProperty(PropertyName = "remove")]
        public IList<string> Remove { get; set; } = default!;

    }

}