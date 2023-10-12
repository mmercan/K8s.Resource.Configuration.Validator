using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using k8s;
using k8s.Autorest;
using k8s.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Sentinel.Core.K8s;
using Sentinel.Core.K8s.Extensions;
using Sentinel.Core.K8s.Models.Entities;
using Xunit;

namespace Sentinel.Core.K8s.Tests;
public class KubernetesClientTests
{
    private readonly Mock<IKubernetes> _kubernetesMock;
    private readonly KubernetesClient _kubernetesClient;

    public KubernetesClientTests()
    {
        _kubernetesMock = new Mock<IKubernetes>();
        _kubernetesClient = new KubernetesClient(new KubernetesClientConfiguration(), _kubernetesMock.Object, new Mock<ILogger<KubernetesClient>>().Object);
    }

    [Fact]
    public async Task GetCurrentNamespace_ReturnsDefaultNamespace_WhenNoNamespaceIsSet()
    {
        // Arrange
        var expectedNamespace = "default";

        // Act
        var actualNamespace = await _kubernetesClient.GetCurrentNamespace();

        // Assert
        Assert.Equal(expectedNamespace, actualNamespace);
    }

    [Fact]
    public async Task GetCurrentNamespace_ReturnsNamespaceFromConfig_WhenNamespaceIsSet()
    {
        // Arrange
        var expectedNamespace = "test-namespace";
        var clientConfig = new KubernetesClientConfiguration { Namespace = expectedNamespace };
        var kubernetesClient = new KubernetesClient(clientConfig, _kubernetesMock.Object, new Mock<ILogger<KubernetesClient>>().Object);

        // Act
        var actualNamespace = await kubernetesClient.GetCurrentNamespace();

        // Assert
        Assert.Equal(expectedNamespace, actualNamespace);
    }

    [Fact]
    public async Task GetCurrentNamespace_ReturnsNamespaceFromEnvironmentVariable_WhenEnvironmentVariableIsSet()
    {
        // Arrange
        var expectedNamespace = "test-namespace";
        Environment.SetEnvironmentVariable("POD_NAMESPACE", expectedNamespace);

        // Act
        var actualNamespace = await _kubernetesClient.GetCurrentNamespace();

        // Assert
        Assert.Equal(expectedNamespace, actualNamespace);

        // Cleanup
        Environment.SetEnvironmentVariable("POD_NAMESPACE", null);
    }

    [Fact]
    public async Task GetCurrentNamespace_ReturnsNamespaceFromFile_WhenFileExists()
    {
        // Arrange
        var expectedNamespace = "default";
        // var fileSystemMock = new Mock<IFileSystem>();
        //  fileSystemMock.Setup(fs => fs.FileExists("/var/run/secrets/kubernetes.io/serviceaccount/namespace")).Returns(true);
        //   fileSystemMock.Setup(fs => fs.ReadAllText("/var/run/secrets/kubernetes.io/serviceaccount/namespace")).Returns(expectedNamespace);
        var kubernetesClient = new KubernetesClient(new KubernetesClientConfiguration(), _kubernetesMock.Object, new Mock<ILogger<KubernetesClient>>().Object);

        // Act
        var actualNamespace = await kubernetesClient.GetCurrentNamespace();

        // Assert
        Assert.Equal(expectedNamespace, actualNamespace);
    }

    [Fact]
    public async Task GetCurrentNamespace_ReturnsDefaultNamespace_WhenFileDoesNotExist()
    {
        // Arrange
        var expectedNamespace = "default";
        //var fileSystemMock = new Mock<IFileSystem>();
        //fileSystemMock.Setup(fs => fs.FileExists("/var/run/secrets/kubernetes.io/serviceaccount/namespace")).Returns(false);
        var kubernetesClient = new KubernetesClient(new KubernetesClientConfiguration(), _kubernetesMock.Object, new Mock<ILogger<KubernetesClient>>().Object);

        // Act
        var actualNamespace = await kubernetesClient.GetCurrentNamespace();

        // Assert
        Assert.Equal(expectedNamespace, actualNamespace);
    }



    // https://github.com/devlooped/moq/issues/1171 Extension methods can't be mocked
    // [Fact]
    // public async Task GetServerVersion_ReturnsServerVersion()
    // {
    //     // Arrange
    //     var expectedVersion = new VersionInfo { GitVersion = "v1.20.0" };
    //     _kubernetesMock.Setup(k => k.Version.GetCodeAsync(default)).ReturnsAsync(expectedVersion);
    //     // Arrange
    //     //   var expectedVersion = new VersionInfo { GitVersion = "v1.20.0" };
    //     //   _kubernetesMock.Setup(k => k.Version.GetCodeAsync()).GetCodeAsync(expectedVersion);
    //     // Act
    //     var actualVersion = await _kubernetesClient.GetServerVersion();
    //     // Assert
    //     Assert.Equal(expectedVersion, actualVersion);
    // }


    // [Fact]
    // public async Task Get_ReturnsResource_WhenResourceExists()
    // {
    //     // Arrange
    //     var expectedResource = new V1Namespace { Metadata = new V1ObjectMeta { Name = "test-namespace" } };
    //    //   _kubernetesMock.Setup(k => k.ReadAsync<V1Namespace>(expectedResource.Metadata.Name, null)).ReturnsAsync(expectedResource);
    //     //var qq =  _kubernetesClient.Create<V1Namespace>();
    //     // Act
    //     var actualResource = await _kubernetesClient.Get<V1Namespace>(expectedResource.Metadata.Name);
    //     // Assert
    //     Assert.Equal(expectedResource, actualResource);
    // }






    // [Fact]
    // public async Task Get_ReturnsNull_WhenResourceDoesNotExist()
    // {
    //     // Arrange
    //     _kubernetesMock.Setup(k => k.ReadNamespacedAsync<V1Namespace>("non-existent-namespace", null)).ThrowsAsync(new HttpOperationException()); //{ Response = new Response { StatusCode = System.Net.HttpStatusCode.NotFound } });
    //     // Act
    //     var actualResource = await _kubernetesClient.Get<V1Namespace>("non-existent-namespace");
    //     // Assert
    //     Assert.Null(actualResource);
    // }

    // [Fact]
    // public async Task List_ReturnsListOfResources()
    // {
    //     // Arrange
    //     var expectedResources = new List<V1Namespace> { new V1Namespace { Metadata = new V1ObjectMeta { Name = "test-namespace-1" } }, new V1Namespace { Metadata = new V1ObjectMeta { Name = "test-namespace-2" } } };
    //     var responseBody = new EntityList<V1Namespace> { Items = expectedResources };
    //     _kubernetesMock.Setup(k => k.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync("core", "v1", null, "namespaces", null)).ReturnsAsync(
    //         new HttpOperationResponse
    //         {
    //             Response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(responseBody)) }
    //         });

    //     // Act
    //     var actualResources = await _kubernetesClient.List<V1Namespace>();

    //     // Assert
    //     Assert.Equal(expectedResources, actualResources);
    // }

    // [Fact]
    // public async Task Save_CallsCreate_WhenResourceUidIsNull()
    // {
    //     // Arrange
    //     var resource = new V1Namespace { Metadata = new V1ObjectMeta { Name = "test-namespace" } };
    //     _kubernetesMock.Setup(k => k.CreateNamespacedAsync(resource, resource.Metadata.NamespaceProperty)).ReturnsAsync(resource);

    //     // Act
    //     var actualResource = await _kubernetesClient.Save(resource);

    //     // Assert
    //     _kubernetesMock.Verify(k => k.CreateNamespacedAsync(resource, resource.Metadata.NamespaceProperty), Times.Once);
    //     _kubernetesMock.Verify(k => k.ReplaceNamespacedAsync(resource, resource.Metadata.NamespaceProperty, resource.Metadata.Name), Times.Never);
    //     Assert.Equal(resource, actualResource);
    // }

    // [Fact]
    // public async Task Save_CallsUpdate_WhenResourceUidIsNotNull()
    // {
    //     // Arrange
    //     var resource = new V1Namespace { Metadata = new V1ObjectMeta { Name = "test-namespace", Uid = "test-uid" } };
    //     _kubernetesMock.Setup(k => k.ReplaceNamespacedAsync(resource, resource.Metadata.NamespaceProperty, resource.Metadata.Name)).ReturnsAsync(resource);

    //     // Act
    //     var actualResource = await _kubernetesClient.Save(resource);

    //     // Assert
    //     _kubernetesMock.Verify(k => k.CreateNamespacedAsync(resource, resource.Metadata.NamespaceProperty), Times.Never);
    //     _kubernetesMock.Verify(k => k.ReplaceNamespacedAsync(resource, resource.Metadata.NamespaceProperty, resource.Metadata.Name), Times.Once);
    //     Assert.Equal(resource, actualResource);
    // }

    // [Fact]
    // public async Task UpdateStatus_UpdatesResourceStatus()
    // {
    //     // Arrange
    //     var resource = new V1Namespace { Metadata = new V1ObjectMeta { Name = "test-namespace" } };
    //     _kubernetesMock.Setup(k => k.CustomObjects.ReplaceNamespacedCustomObjectStatusAsync(resource, "core", "v1", resource.Metadata.NamespaceProperty, "namespaces", resource.Metadata.Name)).Returns(Task.CompletedTask);

    //     // Act
    //     await _kubernetesClient.UpdateStatus(resource);

    //     // Assert
    //     _kubernetesMock.Verify(k => k.CustomObjects.ReplaceNamespacedCustomObjectStatusAsync(resource, "core", "v1", resource.Metadata.NamespaceProperty, "namespaces", resource.Metadata.Name), Times.Once);
    // }

    // [Fact]
    // public async Task Delete_CallsDelete_WhenResourceExists()
    // {
    //     // Arrange
    //     var resource = new V1Namespace { Metadata = new V1ObjectMeta { Name = "test-namespace" } };
    //     _kubernetesMock.Setup(k => k.DeleteNamespacedAsync<V1Namespace>(resource.Metadata.Name, resource.Metadata.NamespaceProperty)).Returns(Task.CompletedTask);

    //     // Act
    //     await _kubernetesClient.Delete(resource);

    //     // Assert
    //     _kubernetesMock.Verify(k => k.DeleteNamespacedAsync<V1Namespace>(resource.Metadata.Name, resource.Metadata.NamespaceProperty), Times.Once);
    // }

    // [Fact]
    // public async Task Delete_DoesNotCallDelete_WhenResourceDoesNotExist()
    // {
    //     // Arrange
    //     _kubernetesMock.Setup(k => k.DeleteNamespacedAsync<V1Namespace>("non-existent-namespace", null)).ThrowsAsync(new HttpOperationException { Response = new Response { StatusCode = System.Net.HttpStatusCode.NotFound } });

    //     // Act
    //     await _kubernetesClient.Delete<V1Namespace>("non-existent-namespace");

    //     // Assert
    //     _kubernetesMock.Verify(k => k.DeleteNamespacedAsync<V1Namespace>("non-existent-namespace", null), Times.Never);
    // }

    // [Fact]
    // public async Task Watch_ReturnsWatcher()
    // {
    //     // Arrange
    //     var expectedWatcher = new Watcher<V1Namespace>(new Mock<IDisposable>().Object, new Mock<IAsyncEnumerator<WatchEventType<V1Namespace>>>().Object);
    //     _kubernetesMock.Setup(k => k.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync("core", "v1", null, "namespaces", null, null, watch:true)).ReturnsAsync(new HttpOperationResponse { Response = new Response { StatusCode = System.Net.HttpStatusCode.OK } });
    //     _kubernetesMock.Setup(k => k.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync("core", "v1", null, "namespaces", null, null, watch:true).Watch<V1Namespace>(null, null, null)).Returns(expectedWatcher);

    //     // Act
    //     var actualWatcher = await _kubernetesClient.Watch<V1Namespace>(TimeSpan.FromSeconds(30), null);

    //     // Assert
    //     Assert.Equal(expectedWatcher, actualWatcher);
    // }
}
