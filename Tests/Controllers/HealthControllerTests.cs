using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SampleDotNet6App.Controllers;
using Xunit;

namespace SampleDotNet6App.Tests.Controllers;

public class HealthControllerTests
{
    private readonly Mock<ILogger<HealthController>> _mockLogger;
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        _mockLogger = new Mock<ILogger<HealthController>>();
        _controller = new HealthController(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_CreatesInstance()
    {
        // Arrange & Act
        var controller = new HealthController(_mockLogger.Object);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public void GetHealth_ReturnsOkResult()
    {
        // Act
        var result = _controller.GetHealth();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetHealth_ReturnsHealthyStatus()
    {
        // Act
        var result = _controller.GetHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var value = result.Value;
        Assert.NotNull(value);
        var statusProperty = value.GetType().GetProperty("status");
        Assert.NotNull(statusProperty);
        Assert.Equal("Healthy", statusProperty.GetValue(value));
    }

    [Fact]
    public void GetHealth_ReturnsTimestamp()
    {
        // Act
        var result = _controller.GetHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var value = result.Value;
        Assert.NotNull(value);
        var timestampProperty = value.GetType().GetProperty("timestamp");
        Assert.NotNull(timestampProperty);
        var timestamp = timestampProperty.GetValue(value);
        Assert.IsType<DateTime>(timestamp);
    }

    [Fact]
    public void GetHealth_ReturnsVersion()
    {
        // Act
        var result = _controller.GetHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var value = result.Value;
        Assert.NotNull(value);
        var versionProperty = value.GetType().GetProperty("version");
        Assert.NotNull(versionProperty);
        Assert.Equal("1.0.0", versionProperty.GetValue(value));
    }

    [Fact]
    public void GetHealth_LogsInformation()
    {
        // Act
        _controller.GetHealth();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Health check requested")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void GetDetailedHealth_ReturnsOkResult()
    {
        // Act
        var result = _controller.GetDetailedHealth();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetDetailedHealth_ReturnsHealthyStatus()
    {
        // Act
        var result = _controller.GetDetailedHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var value = result.Value;
        Assert.NotNull(value);
        var statusProperty = value.GetType().GetProperty("status");
        Assert.NotNull(statusProperty);
        Assert.Equal("Healthy", statusProperty.GetValue(value));
    }

    [Fact]
    public void GetDetailedHealth_ReturnsUptime()
    {
        // Act
        var result = _controller.GetDetailedHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var value = result.Value;
        Assert.NotNull(value);
        var uptimeProperty = value.GetType().GetProperty("uptime");
        Assert.NotNull(uptimeProperty);
        var uptime = uptimeProperty.GetValue(value);
        Assert.IsType<string>(uptime);
    }

    [Fact]
    public void GetDetailedHealth_ReturnsMachineName()
    {
        // Act
        var result = _controller.GetDetailedHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var value = result.Value;
        Assert.NotNull(value);
        var machineProperty = value.GetType().GetProperty("machine");
        Assert.NotNull(machineProperty);
        Assert.NotNull(machineProperty.GetValue(value));
    }

    [Fact]
    public void GetDetailedHealth_ReturnsProcessorCount()
    {
        // Act
        var result = _controller.GetDetailedHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var value = result.Value;
        Assert.NotNull(value);
        var processorCountProperty = value.GetType().GetProperty("processorCount");
        Assert.NotNull(processorCountProperty);
        var processorCount = (int)processorCountProperty.GetValue(value)!;
        Assert.True(processorCount > 0);
    }

    [Fact]
    public void GetDetailedHealth_LogsInformation()
    {
        // Act
        _controller.GetDetailedHealth();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Detailed health check requested")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
