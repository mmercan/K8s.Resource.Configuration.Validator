using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sentinel.Core.K8s.Models.Istio
{
    public class HttpFaultInjection
    {

        /// <summary>
        /// Gets/sets the 
        /// </summary>
        [JsonProperty(PropertyName = "delay")]
        public Delay Delay { get; set; } = default!;

        /// <summary>
        /// Gets/sets the 
        /// </summary>
        [JsonProperty(PropertyName = "abort")]
        public Abort Abort { get; set; } = default!;

        /// <summary>
        /// Validates the specification
        /// </summary>
        public virtual void Validate()
        {
            if (this.Delay == null && this.Abort == null)
                throw new Exception($"The {Delay} property and/or the {Abort} property must be set");
            this.Delay?.Validate();
        }

    }

}