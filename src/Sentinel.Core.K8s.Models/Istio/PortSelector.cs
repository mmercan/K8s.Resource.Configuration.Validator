using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sentinel.Core.K8s.Models.Istio
{
     public class PortSelector
    {

        /// <summary>
        /// Initializes a new <see cref="PortSelector"/>
        /// </summary>
        public PortSelector()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="PortSelector"/>
        /// </summary>
        /// <param name="number">The port number</param>
        public PortSelector(int number)
        {
            this.Number = number;
        }

        /// <summary>
        /// Gets/sets the port number
        /// </summary>
        [JsonProperty(PropertyName = "number")]
        public int Number { get; set; }

    }

}