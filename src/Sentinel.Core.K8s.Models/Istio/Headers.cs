using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sentinel.Core.K8s.Models.Istio
{
    public class Headers
    {

        /// <summary>
        /// Gets/sets the header manipulation rules to apply before forwarding a request to the destination service
        /// </summary>
        [JsonProperty(PropertyName = "request")]
        public HeadersOperations Request { get; set; }= default!;

        /// <summary>
        /// Gets/sets the manipulation rules to apply before returning a response to the caller
        /// </summary>
        [JsonProperty(PropertyName = "response")]
        public HeadersOperations Response { get; set; }= default!;

    }
}