using Xunit;
using SampleDotNet6App.Models;
using System;

namespace SampleDotNet6App.Models.Tests;

public class UserTests
{
    [Fact]
    public void User_CanBeInstantiated()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.NotNull(user);
    }

    [Fact]
    public void User_PropertiesCanBeSet()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash123",
            Role = UserRole.Admin,
            CreatedAt = now,
            IsActive = true
        };

        // Assert
        Assert.Equal(1, user.Id);
        Assert.Equal("testuser", user.Username);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("Test", user.FirstName);
        Assert.Equal("User", user.LastName);
        Assert.Equal("hash123", user.PasswordHash);
        Assert.Equal(UserRole.Admin, user.Role);
        Assert.Equal(now, user.CreatedAt);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_DefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.Username);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(UserRole.User, user.Role);
        Assert.Equal(default(DateTime), user.CreatedAt);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_RoleDefaultsToUser()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(UserRole.User, user.Role);
    }

    [Fact]
    public void User_IsActiveDefaultsToTrue()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.True(user.IsActive);
    }

    [Theory]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Manager)]
    public void User_RoleProperty_AcceptsAllUserRoles(UserRole role)
    {
        // Arrange
        var user = new User { Role = role };

        // Assert
        Assert.Equal(role, user.Role);
    }

    [Theory]
    [InlineData("user1")]
    [InlineData("admin")]
    [InlineData("john.doe")]
    [InlineData("testuser123")]
    public void User_UsernameProperty_AcceptsVariousFormats(string username)
    {
        // Arrange
        var user = new User { Username = username };

        // Assert
        Assert.Equal(username, user.Username);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user@domain.co.uk")]
    [InlineData("admin@company.org")]
    public void User_EmailProperty_AcceptsVariousFormats(string email)
    {
        // Arrange
        var user = new User { Email = email };

        // Assert
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void User_PasswordHashProperty_CanStoreHash()
    {
        // Arrange
        var hash = "hashed_password_string_123";
        var user = new User { PasswordHash = hash };

        // Assert
        Assert.Equal(hash, user.PasswordHash);
    }

    [Fact]
    public void User_IsActiveProperty_CanBeToggled()
    {
        // Arrange
        var user = new User { IsActive = true };

        // Act
        user.IsActive = false;

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void User_AllPropertiesCanBeModified()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "original",
            Email = "original@example.com",
            FirstName = "Original",
            LastName = "Name",
            PasswordHash = "hash1",
            Role = UserRole.User,
            IsActive = true
        };

        // Act
        user.Id = 2;
        user.Username = "modified";
        user.Email = "modified@example.com";
        user.FirstName = "Modified";
        user.LastName = "User";
        user.PasswordHash = "hash2";
        user.Role = UserRole.Admin;
        user.IsActive = false;

        // Assert
        Assert.Equal(2, user.Id);
        Assert.Equal("modified", user.Username);
        Assert.Equal("modified@example.com", user.Email);
        Assert.Equal("Modified", user.FirstName);
        Assert.Equal("User", user.LastName);
        Assert.Equal("hash2", user.PasswordHash);
        Assert.Equal(UserRole.Admin, user.Role);
        Assert.False(user.IsActive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999999)]
    public void User_IdProperty_AcceptsVariousValues(int id)
    {
        // Arrange
        var user = new User { Id = id };

        // Assert
        Assert.Equal(id, user.Id);
    }
}

public class UserRoleTests
{
    [Fact]
    public void UserRole_HasUserValue()
    {
        // Arrange & Act
        var role = UserRole.User;

        // Assert
        Assert.Equal(1, (int)role);
    }

    [Fact]
    public void UserRole_HasAdminValue()
    {
        // Arrange & Act
        var role = UserRole.Admin;

        // Assert
        Assert.Equal(2, (int)role);
    }

    [Fact]
    public void UserRole_HasManagerValue()
    {
        // Arrange & Act
        var role = UserRole.Manager;

        // Assert
        Assert.Equal(3, (int)role);
    }

    [Fact]
    public void UserRole_AllValuesAreDistinct()
    {
        // Arrange
        var user = UserRole.User;
        var admin = UserRole.Admin;
        var manager = UserRole.Manager;

        // Assert
        Assert.NotEqual(user, admin);
        Assert.NotEqual(user, manager);
        Assert.NotEqual(admin, manager);
    }

    [Theory]
    [InlineData(UserRole.User, "User")]
    [InlineData(UserRole.Admin, "Admin")]
    [InlineData(UserRole.Manager, "Manager")]
    public void UserRole_ToStringReturnsCorrectValue(UserRole role, string expected)
    {
        // Arrange & Act
        var result = role.ToString();

        // Assert
        Assert.Equal(expected, result);
    }
}
