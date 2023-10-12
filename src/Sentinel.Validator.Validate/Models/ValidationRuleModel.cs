using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sentinel.Validator.Validate.Models;

public class ValidationRuleModel
{
    public string Name { get; set; } = default!; //: "EnvoyFilter Service hosts is not Empty",
    public string Description { get; set; } = default!; //": "",
    public string jsonPath { get; set; } = default!; //: "spec.configPatches[*].patch.ValueString",
    public string Operator { get; set; } = default!; //": "!contains",
    public IComparable? ExpectedValue { get; set; } = default!; //": "bd4bb8c2d406a1cae684cf045d79e7a0b3afef54ec9258760be2bdcb9918200b",

    public string? ValueDataType { get; set; } = default!; //": "string",
    public string CaptureJsonPath { get; set; } = default!; //": "spec.configPatches[*].patch.ValueString"
}
