using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sentinel.Core.K8s.Models.Istio
{
    public class Delay
    {

        /// <summary>
        /// Gets/sets a fixed delay before forwarding the request. Format: 1h/1m/1s/1ms. MUST be >=1ms. 
        /// </summary>
        [JsonProperty(PropertyName = "fixedDelay")]
        public string FixedDelay { get; set; } = default!;

        /// <summary>
        /// Gets/sets the percentage of requests on which the delay will be injected.
        /// </summary>
        [JsonProperty(PropertyName = "percentage")]
        public Percent Percentage { get; set; } = default!;

        /// <summary>
        /// Gets/sets the percentage of requests on which the delay will be injected (0-100).
        /// </summary>
        [Obsolete("Use of integer percent value is deprecated. Use the double percentage field instead.")]
        [JsonProperty(PropertyName = "percent")]
        public int Percent { get; set; }

        /// <summary>
        /// Validates the specification
        /// </summary>
        public virtual void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.FixedDelay))
                throw new NullReferenceException($"The {FixedDelay} property must be set");
        }

    }
}