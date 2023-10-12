using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sentinel.Validator.Validate.Models
{
    public class PolicyItemValidationResult
    {
        public bool Isvalid { get; init; }
        public string ResourceName { get; init; } = string.Empty;
        public string ResourceNamespace { get; init; } = string.Empty;
        public string ResourceType { get; init; } = string.Empty;
        public string Error { get; init; } = string.Empty;
        public string? CapturedString { get; init; } = string.Empty;
        public ValidationRuleModel Rule { get; init; } = default!;
    }
}