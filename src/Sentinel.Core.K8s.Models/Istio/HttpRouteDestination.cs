using Newtonsoft.Json;

namespace Sentinel.Core.K8s.Models.Istio
{
    public class HttpRouteDestination
    {

        /// <summary>
        /// Gets/sets the destination uniquely identifies the instances of a service to which the request/connection should be forwarded to.
        /// </summary>
        [JsonProperty(PropertyName = "destination")]
        public Destination? Destination { get; set; }

        /// <summary>
        /// Gets/sets the proportion of traffic to be forwarded to the service version. (0-100). Sum of weights across destinations SHOULD BE == 100. If there is only one destination in a rule, the weight value is assumed to be 100.
        /// </summary>
        [JsonProperty(PropertyName = "weight")]
        public int Weight { get; set; }

        /// <summary>
        /// Gets/sets header manipulation rules
        /// </summary>
        [JsonProperty(PropertyName = "headers")]
        public Headers? Headers { get; set; }

        /// <summary>
        /// Validates the specification
        /// </summary>
        public virtual void Validate()
        {
            if (this.Destination == null)
                throw new Exception($"The {Destination} property must be set");
            this.Destination.Validate();
        }

    }
}