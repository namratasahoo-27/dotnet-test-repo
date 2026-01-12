using Xunit;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.DTOs.Tests;

public class UserDtoTests
{
    [Fact]
    public void UserDto_CanBeInstantiated()
    {
        // Arrange & Act
        var dto = new UserDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void UserDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new UserDto
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Role = UserRole.User,
            IsActive = true
        };

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("testuser", dto.Username);
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal("Test", dto.FirstName);
        Assert.Equal("User", dto.LastName);
        Assert.Equal(UserRole.User, dto.Role);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void UserDto_DefaultValues()
    {
        // Arrange & Act
        var dto = new UserDto();

        // Assert
        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Username);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal((UserRole)0, dto.Role);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void LoginDto_CanBeInstantiated()
    {
        // Arrange & Act
        var dto = new LoginDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void LoginDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new LoginDto
        {
            Username = "testuser",
            Password = "password123"
        };

        // Assert
        Assert.Equal("testuser", dto.Username);
        Assert.Equal("password123", dto.Password);
    }

    [Fact]
    public void LoginDto_DefaultValues()
    {
        // Arrange & Act
        var dto = new LoginDto();

        // Assert
        Assert.Equal(string.Empty, dto.Username);
        Assert.Equal(string.Empty, dto.Password);
    }

    [Fact]
    public void RegisterDto_CanBeInstantiated()
    {
        // Arrange & Act
        var dto = new RegisterDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void RegisterDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@example.com",
            FirstName = "New",
            LastName = "User",
            Password = "password123"
        };

        // Assert
        Assert.Equal("newuser", dto.Username);
        Assert.Equal("new@example.com", dto.Email);
        Assert.Equal("New", dto.FirstName);
        Assert.Equal("User", dto.LastName);
        Assert.Equal("password123", dto.Password);
    }

    [Fact]
    public void RegisterDto_DefaultValues()
    {
        // Arrange & Act
        var dto = new RegisterDto();

        // Assert
        Assert.Equal(string.Empty, dto.Username);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal(string.Empty, dto.Password);
    }

    [Fact]
    public void AuthResponseDto_CanBeInstantiated()
    {
        // Arrange & Act
        var dto = new AuthResponseDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void AuthResponseDto_PropertiesCanBeSet()
    {
        // Arrange
        var userDto = new UserDto { Id = 1, Username = "testuser" };
        var dto = new AuthResponseDto
        {
            Token = "test-token-123",
            User = userDto
        };

        // Assert
        Assert.Equal("test-token-123", dto.Token);
        Assert.NotNull(dto.User);
        Assert.Equal(1, dto.User.Id);
        Assert.Equal("testuser", dto.User.Username);
    }

    [Fact]
    public void AuthResponseDto_DefaultValues()
    {
        // Arrange & Act
        var dto = new AuthResponseDto();

        // Assert
        Assert.Equal(string.Empty, dto.Token);
        Assert.NotNull(dto.User);
    }

    [Theory]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Manager)]
    public void UserDto_RoleProperty_AcceptsAllRoles(UserRole role)
    {
        // Arrange
        var dto = new UserDto { Role = role };

        // Assert
        Assert.Equal(role, dto.Role);
    }

    [Theory]
    [InlineData("user1")]
    [InlineData("admin")]
    [InlineData("testuser123")]
    [InlineData("john.doe")]
    public void LoginDto_UsernameProperty_AcceptsVariousFormats(string username)
    {
        // Arrange
        var dto = new LoginDto { Username = username };

        // Assert
        Assert.Equal(username, dto.Username);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user@domain.co.uk")]
    [InlineData("admin@company.org")]
    public void RegisterDto_EmailProperty_AcceptsVariousFormats(string email)
    {
        // Arrange
        var dto = new RegisterDto { Email = email };

        // Assert
        Assert.Equal(email, dto.Email);
    }
}
