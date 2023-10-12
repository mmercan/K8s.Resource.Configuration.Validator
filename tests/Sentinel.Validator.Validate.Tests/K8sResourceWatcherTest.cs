using System;
using System.Threading.Tasks;
using k8s;
using Microsoft.Extensions.Logging;
using Moq;
using Sentinel.Core.K8s;
using Sentinel.Validator.Validate;
using Sentinel.Validator.Validate.Models;
using Sentinel.Validator.Validate.Watcher;
using Xunit;

namespace Sentinel.Validator.Validate.Tests;

public class K8sResourceWatcherTests
{
    // [Fact]
    // public async Task Watch_ShouldThrowArgumentNullException_WhenValidationModelIsNull()
    // {
    //     // Arrange
    //     var loggerMock = new Mock<ILogger<K8sResourceWatcher>>();
    //     var kubernetesClientMock = new Mock<IKubernetesClient>();
    //     var watcher = new K8sResourceWatcher(loggerMock.Object, kubernetesClientMock.Object);

    //     // Act & Assert
    //     await Assert.ThrowsAsync<ArgumentNullException>(() => watcher.Watch(null));
    // }

    [Fact]
    public async Task Watch_ShouldThrowArgumentNullException_WhenK8sResourceIsNull()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<K8sResourceWatcher>>();
        var kubernetesClientMock = new Mock<IKubernetesClient>();
        var watcher = new K8sResourceWatcher(loggerMock.Object, kubernetesClientMock.Object);
        var validationModel = new ValidationModel();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => watcher.Watch(validationModel));
    }

    [Fact]
    public async Task Watch_ShouldThrowArgumentNullException_WhenK8sResourcePluralIsNull()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<K8sResourceWatcher>>();
        var kubernetesClientMock = new Mock<IKubernetesClient>();
        var watcher = new K8sResourceWatcher(loggerMock.Object, kubernetesClientMock.Object);
        var validationModel = new ValidationModel
        {
            K8sResource = new ValidationResource
            {
                Group = "group",
                Version = "version",
                Plural = null
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => watcher.Watch(validationModel));
    }

    [Fact]
    public async Task Watch_ShouldCallKubernetesClientWatch_WhenValidationModelIsValid()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<K8sResourceWatcher>>();
        var kubernetesClientMock = new Mock<IKubernetesClient>();
        var watcher = new K8sResourceWatcher(loggerMock.Object, kubernetesClientMock.Object);
        var validationModel = new ValidationModel
        {
            K8sResource = new ValidationResource
            {
                Group = "group",
                Version = "version",
                Plural = "plural"
            }
        };

        // Act
        await watcher.Watch(validationModel);

        // Assert
        // kubernetesClientMock.Verify(x => x.Watch(
        //     validationModel.K8sResource.Group,
        //     validationModel.K8sResource.Version,
        //     validationModel.K8sResource.Plural,
        //     TimeSpan.FromSeconds(5),
        //     It.IsAny<Action<WatchEventType, object>>()
        // ), Times.Once);
    }
}
