using k8s;
using k8s.Autorest;
using k8s.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Sentinel.Core.K8s.Extensions;
using Sentinel.Core.K8s.Models.Entities;
using Sentinel.Core.K8s.Models.LabelSelectors;
using System.Net;

namespace Sentinel.Core.K8s;

public class KubernetesClient : IKubernetesClient
{
    private const string DownwardApiNamespaceFile = "/var/run/secrets/kubernetes.io/serviceaccount/namespace";
    private const string DefaultNamespace = "default";

    private readonly KubernetesClientConfiguration _clientConfig;
    private readonly IKubernetes _client;
    private readonly ILogger<KubernetesClient> _logger;

    public KubernetesClient(KubernetesClientConfiguration clientConfig, ILogger<KubernetesClient> logger)
        : this(clientConfig, new Kubernetes(clientConfig), logger)
    {
    }

    public KubernetesClient(KubernetesClientConfiguration clientConfig, IKubernetes client, ILogger<KubernetesClient> logger)
    {
        _clientConfig = clientConfig;
        _client = client;
        _logger = logger;
    }

    /// <inheritdoc />
    public IKubernetes ApiClient => _client;

    /// <inheritdoc />
    public Uri BaseUri => _client.BaseUri;

    /// <inheritdoc />
    public Task<string> GetCurrentNamespace(string downwardApiEnvName = "POD_NAMESPACE")
    {
        var result = DefaultNamespace;

        if (_clientConfig.Namespace != null)
        {
            result = _clientConfig.Namespace;
        }

        if (Environment.GetEnvironmentVariable(downwardApiEnvName) != null)
        {
            result = Environment.GetEnvironmentVariable(downwardApiEnvName) ?? string.Empty;
        }

        if (File.Exists(DownwardApiNamespaceFile))
        {
            var ns = File.ReadAllText(DownwardApiNamespaceFile);
            result = ns.Trim();
        }

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<VersionInfo> GetServerVersion() => _client.Version.GetCodeAsync();

    /// <inheritdoc />
    public async Task<TResource?> Get<TResource>(
        string name,
        string? @namespace = null)
        where TResource : class, IKubernetesObject<V1ObjectMeta>
    {
        try
        {
            var client = CreateClient<TResource>();

            return await (string.IsNullOrWhiteSpace(@namespace)
                ? client.ReadAsync<TResource>(name)
                : client.ReadNamespacedAsync<TResource>(@namespace, name));
        }
        catch (HttpOperationException e) when (e.Response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<IList<TResource>> List<TResource>(string? @namespace = null, string? labelSelector = null)
        where TResource : IKubernetesObject<V1ObjectMeta>
    {
        var definition = CustomEntityDefinitionExtensions.CreateResourceDefinition<TResource>();
        var result = await (string.IsNullOrWhiteSpace(@namespace)
            ? _client.CustomObjects.ListClusterCustomObjectWithHttpMessagesAsync(
                definition.KubernetesEntity.Group,
                definition.KubernetesEntity.ApiVersion,
                definition.KubernetesEntity.PluralName,
                labelSelector: labelSelector)
            : _client.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync(
                definition.KubernetesEntity.Group,
                definition.KubernetesEntity.ApiVersion,
                @namespace,
                definition.KubernetesEntity.PluralName,
                labelSelector: labelSelector));
        var list = KubernetesJson.Deserialize<EntityList<TResource>>(result.Body.ToString());
        return list.Items;
    }

    /// <inheritdoc />
    public Task<IList<TResource>> List<TResource>(
        string? @namespace = null,
        params ILabelSelector[] labelSelectors)
        where TResource : IKubernetesObject<V1ObjectMeta> =>
        List<TResource>(@namespace, labelSelectors.ToExpression());

    /// <inheritdoc />
    public Task<TResource> Save<TResource>(TResource resource)
        where TResource : class, IKubernetesObject<V1ObjectMeta> =>
        resource.Uid() is null
            ? Create(resource)
            : Update(resource);

    /// <inheritdoc />
    public async Task<TResource> Create<TResource>(TResource resource)
        where TResource : IKubernetesObject<V1ObjectMeta>
    {
        var client = CreateClient<TResource>();

        return await (string.IsNullOrWhiteSpace(resource.Namespace())
            ? client.CreateAsync(resource)
            : client.CreateNamespacedAsync(resource, resource.Namespace()));
    }

    /// <inheritdoc />
    public async Task<TResource> Update<TResource>(TResource resource)
        where TResource : IKubernetesObject<V1ObjectMeta>
    {
        var client = CreateClient<TResource>();

        return await (string.IsNullOrWhiteSpace(resource.Namespace())
            ? client.ReplaceAsync(resource, resource.Name())
            : client.ReplaceNamespacedAsync(resource, resource.Namespace(), resource.Name()));
    }

    /// <inheritdoc />
    public async Task UpdateStatus<TResource>(TResource resource)
        where TResource : IKubernetesObject<V1ObjectMeta>
    {
        var definition = CustomEntityDefinitionExtensions.CreateResourceDefinition<TResource>();
        await (string.IsNullOrWhiteSpace(resource.Namespace())
            ? _client.CustomObjects.ReplaceClusterCustomObjectStatusAsync(
                resource,
                definition.KubernetesEntity.Group,
                definition.KubernetesEntity.ApiVersion,
                definition.KubernetesEntity.PluralName,
                resource.Name())
            : _client.CustomObjects.ReplaceNamespacedCustomObjectStatusAsync(
                resource,
                definition.KubernetesEntity.Group,
                definition.KubernetesEntity.ApiVersion,
                resource.Namespace(),
                definition.KubernetesEntity.PluralName,
                resource.Name()));
    }

    /// <inheritdoc />
    public Task Delete<TResource>(TResource resource)
        where TResource : IKubernetesObject<V1ObjectMeta> => Delete<TResource>(
        resource.Name(),
        resource.Namespace());

    /// <inheritdoc />
    public Task Delete<TResource>(IEnumerable<TResource> resources)
        where TResource : IKubernetesObject<V1ObjectMeta> =>
        Task.WhenAll(resources.Select(Delete));

    /// <inheritdoc />
    public Task Delete<TResource>(params TResource[] resources)
        where TResource : IKubernetesObject<V1ObjectMeta> =>
        Task.WhenAll(resources.Select(Delete));

    /// <inheritdoc />
    public async Task Delete<TResource>(string name, string? @namespace = null)
        where TResource : IKubernetesObject<V1ObjectMeta>
    {
        var client = CreateClient<TResource>();

        try
        {
            await (string.IsNullOrWhiteSpace(@namespace)
                ? client.DeleteAsync<TResource>(name)
                : client.DeleteNamespacedAsync<TResource>(@namespace, name));
        }
        catch (HttpOperationException e) when (e.Response.StatusCode == HttpStatusCode.NotFound)
        {
        }
    }

    /// <inheritdoc />
    public Task<Watcher<TResource>> Watch<TResource>(
        TimeSpan timeout,
        Action<WatchEventType, TResource> onEvent,
        Action<Exception>? onError = null,
        Action? onClose = null,
        string? @namespace = null,
        CancellationToken cancellationToken = default,
        params ILabelSelector[] labelSelectors)
        where TResource : IKubernetesObject<V1ObjectMeta>
        => Watch(
            timeout,
            onEvent,
            onError,
            onClose,
            @namespace,
            cancellationToken,
            string.Join(",", labelSelectors.Select(l => l.ToExpression())));

    /// <inheritdoc />
    public Task<Watcher<TResource>> Watch<TResource>(
        TimeSpan timeout,
        Action<WatchEventType, TResource> onEvent,
        Action<Exception>? onError = null,
        Action? onClose = null,
        string? @namespace = null,
        CancellationToken cancellationToken = default,
        string? labelSelector = null)
        where TResource : IKubernetesObject<V1ObjectMeta>
    {
        var definition = CustomEntityDefinitionExtensions.CreateResourceDefinition<TResource>();
        var result = string.IsNullOrWhiteSpace(@namespace)
            ? _client.CustomObjects.ListClusterCustomObjectWithHttpMessagesAsync(
                definition.KubernetesEntity.Group,
                definition.KubernetesEntity.ApiVersion,
                definition.KubernetesEntity.PluralName,
                labelSelector: labelSelector,
                timeoutSeconds: (int)timeout.TotalSeconds,
                watch: true,
                cancellationToken: cancellationToken)
            : _client.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync(
                definition.KubernetesEntity.Group,
                definition.KubernetesEntity.ApiVersion,
                @namespace,
                definition.KubernetesEntity.PluralName,
                labelSelector: labelSelector,
                timeoutSeconds: (int)timeout.TotalSeconds,
                watch: true,
                cancellationToken: cancellationToken);

        return Task.FromResult(
            result.Watch(
                onEvent,
                onError,
                onClose));
    }


    public Task<Watcher<object>> Watch(string Group, string ApiVersion, string PluralName,
        TimeSpan timeout,
        Action<WatchEventType, object> onEvent,
        Action<Exception>? onError = null,
        Action? onClose = null,
        string? @namespace = null,
        CancellationToken cancellationToken = default,
        string? labelSelector = null)
    {


        var result = string.IsNullOrWhiteSpace(@namespace)
            ? _client.CustomObjects.ListClusterCustomObjectWithHttpMessagesAsync(
                Group,
                ApiVersion,
                PluralName,
                labelSelector: labelSelector,
                timeoutSeconds: (int)timeout.TotalSeconds,
                watch: true,
                cancellationToken: cancellationToken)
            : _client.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync(
                Group,
                ApiVersion,
                @namespace,
                PluralName,
                labelSelector: labelSelector,
                timeoutSeconds: (int)timeout.TotalSeconds,
                watch: true,
                cancellationToken: cancellationToken);

        // return await result.Watch(onEvent,onError, onClose);

        return Task.FromResult(

            // result.Result.Watch(onEvent,onError, onClose)
            result.Watch(onEvent, onError, onClose)
         // result.Watch(
         //     onEvent,
         //     onError,
         //     onClose)
         );
    }

    public async Task<List<JToken>> ListClusterCustomObject(string Group, string Version, string Plural)
    {
        var result = await _client.CustomObjects.ListClusterCustomObjectWithHttpMessagesAsync(Group, Version, Plural);

        var jToken = JToken.Parse(result.Body.ToString());
        var res = jToken?.SelectToken("items")?.ToList();
        if (res == null)
        {
            res = new List<JToken>();
        }
        return res;

        //var list = KubernetesJson.Deserialize<EntityList<JToken>>(result.Body.ToString());
        //return list.Items;
    }

    public async Task<IList<V1Namespace>> ListNamespaces()
    {
        var ns = await ApiClient.CoreV1.ListNamespaceAsync();
        return ns.Items;
    }

    private GenericClient CreateClient<TResource>()
        where TResource : IKubernetesObject<V1ObjectMeta>
    {
        var definition = CustomEntityDefinitionExtensions.CreateResourceDefinition<TResource>();
        return definition.KubernetesEntity.Group switch
        {
            "" => new GenericClient(_client, definition.KubernetesEntity.ApiVersion, definition.KubernetesEntity.PluralName),
            _ => new GenericClient(_client, definition.KubernetesEntity.Group, definition.KubernetesEntity.ApiVersion, definition.KubernetesEntity.PluralName),
        };
    }
}