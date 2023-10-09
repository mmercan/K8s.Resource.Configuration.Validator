using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sentinel.Core.K8s.Models.Istio
{
    public class EnvoyFilterSpec
    {
        [JsonProperty(PropertyName = "configPatches")]
        public List<ConfigPatch> ConfigPatches { get; set; } = default!;


        public class ConfigPatch
        {
            [JsonProperty(PropertyName = "applyTo")]
            public string ApplyTo { get; set; } = default!;


            [JsonProperty(PropertyName = "match")]
            public object Match { get; set; } = default!;


            [JsonProperty(PropertyName = "patch")]
            public Patch Patch { get; set; } = default!;

        }


        public class Patch
        {
            [JsonProperty(PropertyName = "operation")]
            public string Operation { get; set; } = default!;

            [JsonProperty("value")]
            public object Value { get; set; }

            public string ValueString { get; set; } = default!;
        }

        public partial class Value
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("typed_config")]
            public TypedConfig TypedConfig { get; set; }
        }

        public partial class TypedConfig
        {
            [JsonProperty("@type")]
            public string Type { get; set; }

            [JsonProperty("inlineCode")]
            public string InlineCode { get; set; }
        }




        public class PatchValueTypedConfig
        {
            [JsonProperty(PropertyName = "@type")]
            public string ValueType { get; set; } = default!;

            [JsonProperty(PropertyName = "inlineCode")]
            public string InlineCode { get; set; } = default!;
        }
    }



}