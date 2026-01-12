using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SampleDotNet6App.Controllers;
using System;

namespace SampleDotNet6App.Controllers.Tests;

public class HealthControllerTests
{
    private readonly Mock<ILogger<HealthController>> _loggerMock;
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        _loggerMock = new Mock<ILogger<HealthController>>();
        _controller = new HealthController(_loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_CreatesInstance()
    {
        // Arrange & Act
        var controller = new HealthController(_loggerMock.Object);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public void Constructor_WithNullLogger_CreatesInstance()
    {
        // Arrange, Act & Assert
        var controller = new HealthController(null!);
        Assert.NotNull(controller);
    }

    [Fact]
    public void GetHealth_ReturnsOkResult()
    {
        // Arrange & Act
        var result = _controller.GetHealth();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetHealth_ReturnsHealthyStatus()
    {
        // Arrange & Act
        var result = _controller.GetHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var resultValue = result.Value;
        var statusProperty = resultValue.GetType().GetProperty("status");
        Assert.NotNull(statusProperty);
        Assert.Equal("Healthy", statusProperty.GetValue(resultValue));
    }

    [Fact]
    public void GetHealth_ReturnsTimestamp()
    {
        // Arrange & Act
        var result = _controller.GetHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var resultValue = result.Value;
        var timestampProperty = resultValue.GetType().GetProperty("timestamp");
        Assert.NotNull(timestampProperty);
        var timestamp = timestampProperty.GetValue(resultValue);
        Assert.IsType<DateTime>(timestamp);
    }

    [Fact]
    public void GetHealth_ReturnsVersion()
    {
        // Arrange & Act
        var result = _controller.GetHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var resultValue = result.Value;
        var versionProperty = resultValue.GetType().GetProperty("version");
        Assert.NotNull(versionProperty);
        Assert.Equal("1.0.0", versionProperty.GetValue(resultValue));
    }

    [Fact]
    public void GetHealth_ReturnsEnvironment()
    {
        // Arrange & Act
        var result = _controller.GetHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var resultValue = result.Value;
        var environmentProperty = resultValue.GetType().GetProperty("environment");
        Assert.NotNull(environmentProperty);
        Assert.NotNull(environmentProperty.GetValue(resultValue));
    }

    [Fact]
    public void GetDetailedHealth_ReturnsOkResult()
    {
        // Arrange & Act
        var result = _controller.GetDetailedHealth();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetDetailedHealth_ReturnsHealthyStatus()
    {
        // Arrange & Act
        var result = _controller.GetDetailedHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var resultValue = result.Value;
        var statusProperty = resultValue.GetType().GetProperty("status");
        Assert.NotNull(statusProperty);
        Assert.Equal("Healthy", statusProperty.GetValue(resultValue));
    }

    [Fact]
    public void GetDetailedHealth_ReturnsUptime()
    {
        // Arrange & Act
        var result = _controller.GetDetailedHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var resultValue = result.Value;
        var uptimeProperty = resultValue.GetType().GetProperty("uptime");
        Assert.NotNull(uptimeProperty);
        Assert.IsType<string>(uptimeProperty.GetValue(resultValue));
    }

    [Fact]
    public void GetDetailedHealth_ReturnsMachineName()
    {
        // Arrange & Act
        var result = _controller.GetDetailedHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var resultValue = result.Value;
        var machineProperty = resultValue.GetType().GetProperty("machine");
        Assert.NotNull(machineProperty);
        Assert.Equal(Environment.MachineName, machineProperty.GetValue(resultValue));
    }

    [Fact]
    public void GetDetailedHealth_ReturnsOsVersion()
    {
        // Arrange & Act
        var result = _controller.GetDetailedHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var resultValue = result.Value;
        var osVersionProperty = resultValue.GetType().GetProperty("osVersion");
        Assert.NotNull(osVersionProperty);
        Assert.NotNull(osVersionProperty.GetValue(resultValue));
    }

    [Fact]
    public void GetDetailedHealth_ReturnsDotnetVersion()
    {
        // Arrange & Act
        var result = _controller.GetDetailedHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var resultValue = result.Value;
        var dotnetVersionProperty = resultValue.GetType().GetProperty("dotnetVersion");
        Assert.NotNull(dotnetVersionProperty);
        Assert.Equal(Environment.Version.ToString(), dotnetVersionProperty.GetValue(resultValue));
    }

    [Fact]
    public void GetDetailedHealth_ReturnsWorkingSet()
    {
        // Arrange & Act
        var result = _controller.GetDetailedHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var resultValue = result.Value;
        var workingSetProperty = resultValue.GetType().GetProperty("workingSet");
        Assert.NotNull(workingSetProperty);
        Assert.IsType<long>(workingSetProperty.GetValue(resultValue));
    }

    [Fact]
    public void GetDetailedHealth_ReturnsProcessorCount()
    {
        // Arrange & Act
        var result = _controller.GetDetailedHealth() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var resultValue = result.Value;
        var processorCountProperty = resultValue.GetType().GetProperty("processorCount");
        Assert.NotNull(processorCountProperty);
        Assert.Equal(Environment.ProcessorCount, processorCountProperty.GetValue(resultValue));
    }
}
