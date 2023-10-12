using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Sentinel.Validator.Validate.Models;

namespace Sentinel.Validator.Validate.Validator
{
    public static class K8sResourceValidator
    {
        public static List<PolicyItemValidationResult> ValidateRules(k8s.WatchEventType eventType, JToken resource, ValidationModel validationModel)
        {
            List<PolicyItemValidationResult> allResults = new List<PolicyItemValidationResult>();
            var resourceName = resource.SelectToken("$.metadata.name")?.ToString();
            var resurceType = resource.SelectToken("$.kind")?.ToString();
            var resourceNamespace = resource.SelectToken("$.metadata.namespace")?.ToString();


            foreach (var rule in validationModel.Validations)
            {
                var isvalid = ValidateRule(resource, rule.jsonPath, rule.Operator, rule.ExpectedValue);
                var capturedString = CaptureJsonPath(resource, rule);

                var result = new PolicyItemValidationResult()
                {
                    Rule = rule,
                    Isvalid = isvalid,
                    ResourceName = resourceName,
                    ResourceType = resurceType,
                    ResourceNamespace = resourceNamespace,
                    CapturedString = capturedString

                };
                allResults.Add(result);
            }
            return allResults;
        }

        private static bool ValidateRule(JToken token, string jsonPath, string Operator, IComparable ExpectedValue)
        {
            var ops = Operator switch
            {
                "" => ValidateComparisonOperator.Equal,
                "==" => ValidateComparisonOperator.Equal,
                "!=" => ValidateComparisonOperator.NotEqual,
                ">" => ValidateComparisonOperator.GreaterThan,
                "<" => ValidateComparisonOperator.LessThan,
                ">=" => ValidateComparisonOperator.GreaterThanOrEqual,
                "<=" => ValidateComparisonOperator.LessThanOrEqual,
                "null" => ValidateComparisonOperator.Null,
                "!null" => ValidateComparisonOperator.NotNull,
                "contains" => ValidateComparisonOperator.Contains,
                "!contains" => ValidateComparisonOperator.NotContains,
                _ => throw new ArgumentException("Operator didn't Match please use one of ==, !=, >, <, >=, <=")
            };
            return Validate(token, jsonPath, ops, ExpectedValue);
        }

        public static bool Validate(JToken token, string jsonPath, ValidateComparisonOperator Operator, IComparable ExpectedValue)
        {

            IEnumerable<JToken> tokens = token.SelectTokens(jsonPath);
            foreach (var tokenSection in tokens)
            {
                //           JToken getnulls = token.SelectToken(jsonPath);
                if (Operator == ValidateComparisonOperator.Null)
                {
                    if (tokenSection is null) return true;
                    return tokenSection.ToString() == "";
                }
                else if (Operator == ValidateComparisonOperator.NotNull)
                {
                    if (tokenSection is null) return false;
                    return tokenSection.ToString() != "";
                }
                var extectedType = ExpectedValue.GetType();
                var convertedValue = Convert.ChangeType(tokenSection.ToString(), extectedType);
                var res = (convertedValue as IComparable).CompareTo(ExpectedValue);

                if (Operator == ValidateComparisonOperator.Contains)
                {
                    if (tokenSection is null) return false;
                    var isvalid = tokenSection.ToString().Contains(ExpectedValue.ToString());
                    return isvalid;
                }
                else if (Operator == ValidateComparisonOperator.NotContains)
                {
                    if (tokenSection is null) return true;
                    var isvalid = tokenSection.ToString().Contains(ExpectedValue.ToString());
                    return !isvalid;
                }

                var ops = Operator switch
                {
                    ValidateComparisonOperator.Equal => res == 0,
                    ValidateComparisonOperator.NotEqual => res != 0,
                    ValidateComparisonOperator.GreaterThan => res > 0,
                    ValidateComparisonOperator.LessThan => res < 0,
                    ValidateComparisonOperator.GreaterThanOrEqual => res <= 0,
                    ValidateComparisonOperator.LessThanOrEqual => res >= 0,
                };
                if (ops == false) return ops;
            }
            return true;
        }

        private static string? CaptureJsonPath(JToken token, ValidationRuleModel rule)
        {
            if (!string.IsNullOrWhiteSpace(rule.CaptureJsonPath))
            {
                var content = token.SelectTokens(rule.CaptureJsonPath);
                if (content.Count() > 0)
                {
                    var capturedString = string.Join(',', content.Select(p => p.ToString()));
                    return capturedString;
                }
            }
            return null;
        }


    }
}
