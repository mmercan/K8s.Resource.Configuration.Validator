using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sentinel.Core.K8s.Models.Istio
{
      public class Percent
    {

        /// <summary>
        /// Initializes a new <see cref="Percent"/>
        /// </summary>
        public Percent()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="Percent"/>
        /// </summary>
        /// <param name="value">The percentage's value</param>
        public Percent(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets/sets the percentage's value
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public double Value { get; set; }

    }
}