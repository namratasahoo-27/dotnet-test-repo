using SampleDotNet6App.DTOs;
using SampleDotNet6App.Models;
using Xunit;

namespace SampleDotNet6App.Tests.DTOs;

public class UserDtoTests
{
    [Fact]
    public void UserDto_DefaultConstructor_CreatesInstance()
    {
        // Act
        var dto = new UserDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Username);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal((UserRole)0, dto.Role);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void UserDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.Id = 1;
        dto.Username = "testuser";
        dto.Email = "test@test.com";
        dto.FirstName = "Test";
        dto.LastName = "User";
        dto.Role = UserRole.Admin;
        dto.IsActive = true;

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("testuser", dto.Username);
        Assert.Equal("test@test.com", dto.Email);
        Assert.Equal("Test", dto.FirstName);
        Assert.Equal("User", dto.LastName);
        Assert.Equal(UserRole.Admin, dto.Role);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void UserDto_WithManagerRole_SetsRole()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.Role = UserRole.Manager;

        // Assert
        Assert.Equal(UserRole.Manager, dto.Role);
    }

    [Fact]
    public void LoginDto_DefaultConstructor_CreatesInstance()
    {
        // Act
        var dto = new LoginDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.Username);
        Assert.Equal(string.Empty, dto.Password);
    }

    [Fact]
    public void LoginDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange
        var dto = new LoginDto();

        // Act
        dto.Username = "testuser";
        dto.Password = "password123";

        // Assert
        Assert.Equal("testuser", dto.Username);
        Assert.Equal("password123", dto.Password);
    }

    [Fact]
    public void RegisterDto_DefaultConstructor_CreatesInstance()
    {
        // Act
        var dto = new RegisterDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.Username);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal(string.Empty, dto.Password);
    }

    [Fact]
    public void RegisterDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange
        var dto = new RegisterDto();

        // Act
        dto.Username = "newuser";
        dto.Email = "new@test.com";
        dto.FirstName = "New";
        dto.LastName = "User";
        dto.Password = "password123";

        // Assert
        Assert.Equal("newuser", dto.Username);
        Assert.Equal("new@test.com", dto.Email);
        Assert.Equal("New", dto.FirstName);
        Assert.Equal("User", dto.LastName);
        Assert.Equal("password123", dto.Password);
    }

    [Fact]
    public void AuthResponseDto_DefaultConstructor_CreatesInstance()
    {
        // Act
        var dto = new AuthResponseDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.Token);
        Assert.NotNull(dto.User);
    }

    [Fact]
    public void AuthResponseDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange
        var dto = new AuthResponseDto();
        var userDto = new UserDto { Id = 1, Username = "testuser" };

        // Act
        dto.Token = "test-token-123";
        dto.User = userDto;

        // Assert
        Assert.Equal("test-token-123", dto.Token);
        Assert.Equal(userDto, dto.User);
        Assert.Equal(1, dto.User.Id);
    }

    [Fact]
    public void AuthResponseDto_WithEmptyToken_SetsToken()
    {
        // Arrange
        var dto = new AuthResponseDto();

        // Act
        dto.Token = "";

        // Assert
        Assert.Equal("", dto.Token);
    }

    [Fact]
    public void AuthResponseDto_WithLongToken_SetsToken()
    {
        // Arrange
        var dto = new AuthResponseDto();
        var longToken = new string('a', 500);

        // Act
        dto.Token = longToken;

        // Assert
        Assert.Equal(longToken, dto.Token);
    }
}
