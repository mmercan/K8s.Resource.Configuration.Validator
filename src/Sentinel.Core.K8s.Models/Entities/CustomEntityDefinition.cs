namespace Sentinel.Core.K8s.Models.Entities;

public readonly struct CustomEntityDefinition
{
    public readonly string Kind;

    public readonly string ListKind;

    public readonly string Group;

    public readonly string ApiVersion;

    public readonly string PluralName;

    public readonly EntityScope Scope;

    public CustomEntityDefinition(
        string kind,
        string listKind,
        string @group,
        string apiVersion,
        string pluralName,
        EntityScope scope)
    {
        Kind = kind;
        ListKind = listKind;
        Group = @group;
        ApiVersion = apiVersion;
        PluralName = pluralName;
        Scope = scope;
    }
}
