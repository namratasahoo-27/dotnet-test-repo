using SampleDotNet6App.Models;
using Xunit;

namespace SampleDotNet6App.Tests.Models;

public class UserTests
{
    [Fact]
    public void User_DefaultConstructor_CreatesInstance()
    {
        // Act
        var user = new User();

        // Assert
        Assert.NotNull(user);
        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.Username);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(UserRole.User, user.Role);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_SetProperties_ReturnsCorrectValues()
    {
        // Arrange
        var user = new User();
        var now = DateTime.UtcNow;

        // Act
        user.Id = 1;
        user.Username = "testuser";
        user.Email = "test@test.com";
        user.FirstName = "Test";
        user.LastName = "User";
        user.PasswordHash = "hashed_password";
        user.Role = UserRole.Admin;
        user.CreatedAt = now;
        user.IsActive = false;

        // Assert
        Assert.Equal(1, user.Id);
        Assert.Equal("testuser", user.Username);
        Assert.Equal("test@test.com", user.Email);
        Assert.Equal("Test", user.FirstName);
        Assert.Equal("User", user.LastName);
        Assert.Equal("hashed_password", user.PasswordHash);
        Assert.Equal(UserRole.Admin, user.Role);
        Assert.Equal(now, user.CreatedAt);
        Assert.False(user.IsActive);
    }

    [Fact]
    public void User_DefaultRole_IsUser()
    {
        // Act
        var user = new User();

        // Assert
        Assert.Equal(UserRole.User, user.Role);
    }

    [Fact]
    public void User_SetRoleToAdmin_UpdatesRole()
    {
        // Arrange
        var user = new User();

        // Act
        user.Role = UserRole.Admin;

        // Assert
        Assert.Equal(UserRole.Admin, user.Role);
    }

    [Fact]
    public void User_SetRoleToManager_UpdatesRole()
    {
        // Arrange
        var user = new User();

        // Act
        user.Role = UserRole.Manager;

        // Assert
        Assert.Equal(UserRole.Manager, user.Role);
    }

    [Fact]
    public void User_DefaultIsActive_IsTrue()
    {
        // Act
        var user = new User();

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_SetIsActiveToFalse_UpdatesProperty()
    {
        // Arrange
        var user = new User { IsActive = true };

        // Act
        user.IsActive = false;

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void User_WithEmptyUsername_SetsUsername()
    {
        // Arrange
        var user = new User();

        // Act
        user.Username = "";

        // Assert
        Assert.Equal("", user.Username);
    }

    [Fact]
    public void User_WithEmptyEmail_SetsEmail()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = "";

        // Assert
        Assert.Equal("", user.Email);
    }

    [Fact]
    public void User_WithLongUsername_SetsUsername()
    {
        // Arrange
        var user = new User();
        var longUsername = new string('a', 100);

        // Act
        user.Username = longUsername;

        // Assert
        Assert.Equal(longUsername, user.Username);
    }

    [Fact]
    public void User_WithValidEmail_SetsEmail()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = "valid.email@example.com";

        // Assert
        Assert.Equal("valid.email@example.com", user.Email);
    }

    [Fact]
    public void User_WithPasswordHash_SetsPasswordHash()
    {
        // Arrange
        var user = new User();
        var hash = "abc123hash";

        // Act
        user.PasswordHash = hash;

        // Assert
        Assert.Equal(hash, user.PasswordHash);
    }

    [Fact]
    public void User_CreatedAt_CanBeSet()
    {
        // Arrange
        var user = new User();
        var timestamp = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        user.CreatedAt = timestamp;

        // Assert
        Assert.Equal(timestamp, user.CreatedAt);
    }

    [Fact]
    public void UserRole_HasCorrectValues()
    {
        // Assert
        Assert.Equal(1, (int)UserRole.User);
        Assert.Equal(2, (int)UserRole.Admin);
        Assert.Equal(3, (int)UserRole.Manager);
    }

    [Fact]
    public void UserRole_User_IsDefinedCorrectly()
    {
        // Act
        var role = UserRole.User;

        // Assert
        Assert.Equal(1, (int)role);
        Assert.Equal("User", role.ToString());
    }

    [Fact]
    public void UserRole_Admin_IsDefinedCorrectly()
    {
        // Act
        var role = UserRole.Admin;

        // Assert
        Assert.Equal(2, (int)role);
        Assert.Equal("Admin", role.ToString());
    }

    [Fact]
    public void UserRole_Manager_IsDefinedCorrectly()
    {
        // Act
        var role = UserRole.Manager;

        // Assert
        Assert.Equal(3, (int)role);
        Assert.Equal("Manager", role.ToString());
    }
}
