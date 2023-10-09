using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sentinel.Core.K8s.Models.Istio
{
    public class StringMatch
    {

        /// <summary>
        /// Gets/sets the exact string match
        /// </summary>
        [JsonProperty(PropertyName = "exact")]
        public string Exact { get; set; } = default!;

        /// <summary>
        /// Gets/sets the prefix-based match
        /// </summary>
        [JsonProperty(PropertyName = "prefix")]
        public string Prefix { get; set; } = default!;

        /// <summary>
        /// Gets/sets the RE2 style regex-based match (<see href="https://github.com/google/re2/wiki/Syntax"/>)
        /// </summary>
        [JsonProperty(PropertyName = "regex")]
        public string Regex { get; set; } = default!;

    }
}