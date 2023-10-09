namespace Sentinel.Core.K8s.Models.LabelSelectors;

public record NotExistsSelector : ILabelSelector
{
    public NotExistsSelector(string label) => Label = label;

    public string Label { get; }

    public string ToExpression() => $"!{Label}";
}
