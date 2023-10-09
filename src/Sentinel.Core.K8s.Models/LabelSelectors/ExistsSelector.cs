namespace Sentinel.Core.K8s.Models.LabelSelectors;

public record ExistsSelector : ILabelSelector
{
    public ExistsSelector(string label) => Label = label;

    public string Label { get; }

    public string ToExpression() => $"{Label}";
}
