using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Sentinel.Core.K8s.Models.Entities;

namespace Sentinel.Core.K8s.Extensions;

public static class CustomEntityDefinitionExtensions
{
    /// <summary>
    /// Create a custom entity definition.
    /// </summary>
    /// <param name="resource">The resource that is used as the type.</param>
    /// <returns>A <see cref="CustomEntityDefinition"/>.</returns>
    public static (KubernetesEntityAttribute KubernetesEntity, EntityScope Scope) CreateResourceDefinition(
        this IKubernetesObject<V1ObjectMeta> resource) =>
        CreateResourceDefinition(resource.GetType());

    /// <summary>
    /// Create a custom entity definition.
    /// </summary>
    /// <typeparam name="TResource">The concrete type of the resource.</typeparam>
    /// <returns>A <see cref="CustomEntityDefinition"/>.</returns>
    public static (KubernetesEntityAttribute KubernetesEntity, EntityScope Scope) CreateResourceDefinition<TResource>()
        where TResource : IKubernetesObject<V1ObjectMeta> =>
        CreateResourceDefinition(typeof(TResource));

    /// <summary>
    /// Create a custom entity definition.
    /// </summary>
    /// <param name="resourceType">A type to construct the definition from.</param>
    /// <exception cref="ArgumentException">
    /// When the type of the resource does not contain a <see cref="KubernetesEntityAttribute"/>.
    /// </exception>
    /// <returns>A <see cref="CustomEntityDefinition"/>.</returns>
    public static (KubernetesEntityAttribute KubernetesEntity, EntityScope Scope) CreateResourceDefinition(this Type resourceType)
    {
        var attribute = resourceType.GetCustomAttribute<KubernetesEntityAttribute>();
        if (attribute == null)
        {
            throw new ArgumentException($"The Type {resourceType} does not have the kubernetes entity attribute.");
        }

        //return attribute;

        var scopeAttribute = resourceType.GetCustomAttribute<EntityScopeAttribute>();
        // var kind = string.IsNullOrWhiteSpace(attribute.Kind) ? resourceType.Name : attribute.Kind;

        return (KubernetesEntity: attribute, Scope: scopeAttribute?.Scope ?? default);
        //Tuple.Create(attribute, scopeAttribute?.Scope ?? default);



    }



    public static KubernetesEntityAttribute CreateCustomResourceDefinition<TResource>(string Namespace)
        where TResource : IKubernetesObject<V1ObjectMeta> =>
        CreateCustomResourceDefinition(typeof(TResource), Namespace);
    public static KubernetesEntityAttribute CreateCustomResourceDefinition(this Type resourceType, string Namespace)
    {
        var attribute = resourceType.GetCustomAttribute<KubernetesEntityAttribute>();
        if (attribute == null)
        {
            throw new ArgumentException($"The Type {resourceType} does not have the kubernetes entity attribute.");
        }
        var kind = string.IsNullOrWhiteSpace(attribute.Kind) ? resourceType.Name : attribute.Kind;
        return attribute;

        // return new CustomResourceDefinition
        // {
        //     Kind = kind,
        //     Group = attribute.Group,
        //     Version = attribute.ApiVersion,
        //     PluralName = string.IsNullOrWhiteSpace(attribute.PluralName) ? $"{kind.ToLower()}s" : attribute.PluralName,
        //     Namespace = Namespace
        // };
    }
}
