using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sentinel.Validator.Validate.Models;
public class ValidationModel
{

    // [JsonPropertyName("Group")]
    // [JsonProperty("Group")]


    [Key]
    public string Name { get; set; } = default!; //: "EnvoyFilter Service hosts is not Empty",

    public ValidationResource K8sResource { get; set; } = default!;

    public string? Schedule { get; set; } = default!;
    public IList<ValidationRuleModel> Validations { get; set; } = default!;
}
