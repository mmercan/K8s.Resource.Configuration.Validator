using System.Collections.Generic;

namespace Sentinel.Core.K8s.Models.LabelSelectors;

public record NotEqualsSelector : ILabelSelector
{
    public NotEqualsSelector(string label, params string[] values) => (Label, Values) = (label, values);

    public string Label { get; set; }

    public IEnumerable<string> Values { get; set; }

    public string ToExpression() => $"{Label} notin ({string.Join(",", Values)})";
}
