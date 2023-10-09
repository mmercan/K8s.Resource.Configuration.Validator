namespace Sentinel.Core.K8s.Models.LabelSelectors;

public interface ILabelSelector
{
    /// <summary>
    /// Create an expresion from the label selector.
    /// </summary>
    /// <returns>A string that represents the label selector.</returns>
    string ToExpression();
}
