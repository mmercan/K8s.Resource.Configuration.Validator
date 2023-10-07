using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sentinel.Validator.POC.Models
{
    public class ValidationResource
    {

        [JsonPropertyName("Group")]
        [JsonProperty("Group")]
        public string Group { get; set; } = "";// "Group": "core",
        public string Plural { get; set; } = default!;// "Plural": "namespaces",
        public string Version { get; set; } = default!;// "Version": "v1",
        public string KubeKind { get; set; } = default!;// "KubeKind": "Namespace",   

    }
}