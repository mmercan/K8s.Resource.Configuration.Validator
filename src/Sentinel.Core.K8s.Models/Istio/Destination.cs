using Newtonsoft.Json;

namespace Sentinel.Core.K8s.Models.Istio
{
    public class Destination
    {

        /// <summary>
        /// Initializes a new <see cref="Destination"/>
        /// </summary>
        public Destination()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Destination"/>
        /// </summary>
        /// <param name="host">The name of a service from the service registry</param>
        /// <param name="subset">The name of a subset within the service</param>
        /// <param name="port">The port on the host that is being addressed</param>
        public Destination(string? host, string? subset, PortSelector? port)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException(nameof(host));
            this.Host = host;
            this.Subset = subset;
            this.Port = port;
        }

        /// <summary>
        /// Initializes a new <see cref="Destination"/>
        /// </summary>
        /// <param name="host">The name of a service from the service registry</param>
        /// <param name="subset">The name of a subset within the service</param>
        public Destination(string host, string subset)
            : this(host, subset, null)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Destination"/>
        /// </summary>
        /// <param name="host">The name of a service from the service registry</param>
        public Destination(string host)
            : this(host, null, null)
        {

        }

        /// <summary>
        /// Gets/sets the name of a service from the service registry. Service names are looked up from the platform’s service registry (e.g., Kubernetes services, Consul services, etc.) and from the hosts declared by ServiceEntry. 
        /// Traffic forwarded to destinations that are not found in either of the two, will be dropped.
        /// </summary>
        [JsonProperty(PropertyName = "host")]
        public string? Host { get; set; }

        /// <summary>
        /// Gets/sets the name of a subset within the service. Applicable only to services within the mesh. The subset must be defined in a corresponding DestinationRule.
        /// </summary>
        [JsonProperty(PropertyName = "subset")]
        public string? Subset { get; set; }

        /// <summary>
        /// Gets/sets the port on the host that is being addressed. If a service exposes only a single port it is not required to explicitly select the port.
        /// </summary>
        [JsonProperty(PropertyName = "port")]
        public PortSelector? Port { get; set; }

        /// <summary>
        /// Validates the specification
        /// </summary>
        public virtual void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.Host))
                throw new Exception($"The {Host} property must be set");
        }

    }
}