using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Services;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Mappings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleDotNet6App.Services.Tests;

public class UserServiceTests
{
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly IMapper _mapper;
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

    public UserServiceTests()
    {
        _loggerMock = new Mock<ILogger<UserService>>();
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(x => x["Jwt:Key"]).Returns("test-key-for-jwt-token-generation-must-be-long-enough-256-bits");

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
    }

    private ApplicationDbContext CreateContext()
    {
        return new ApplicationDbContext(_dbOptions);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task RegisterAsync_CreatesAndReturnsUser()
    {
        // Arrange
        using var context = CreateContext();
        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@example.com",
            FirstName = "New",
            LastName = "User",
            Password = "password123"
        };

        // Act
        var result = await service.RegisterAsync(registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newuser", result.Username);
        Assert.Equal("new@example.com", result.Email);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsActiveUsers()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.AddRange(
            new User { Id = 1, Username = "user1", Email = "user1@example.com", IsActive = true, PasswordHash = "hash1" },
            new User { Id = 2, Username = "user2", Email = "user2@example.com", IsActive = true, PasswordHash = "hash2" },
            new User { Id = 3, Username = "user3", Email = "user3@example.com", IsActive = false, PasswordHash = "hash3" }
        );
        await context.SaveChangesAsync();

        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Id = 1, Username = "testuser", Email = "test@example.com", IsActive = true, PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);

        // Act
        var result = await service.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);

        // Act
        var result = await service.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithValidUsername_ReturnsUser()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Id = 1, Username = "testuser", Email = "test@example.com", IsActive = true, PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);

        // Act
        var result = await service.GetUserByUsernameAsync("testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithInvalidUsername_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);

        // Act
        var result = await service.GetUserByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_OnlyReturnsActiveUsers()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Id = 1, Username = "inactiveuser", Email = "inactive@example.com", IsActive = false, PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);

        // Act
        var result = await service.GetUserByUsernameAsync("inactiveuser");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task IsUsernameAvailableAsync_WithAvailableUsername_ReturnsTrue()
    {
        // Arrange
        using var context = CreateContext();
        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);

        // Act
        var result = await service.IsUsernameAvailableAsync("newuser");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsUsernameAvailableAsync_WithTakenUsername_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Id = 1, Username = "existinguser", Email = "existing@example.com", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);

        // Act
        var result = await service.IsUsernameAvailableAsync("existinguser");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsEmailAvailableAsync_WithAvailableEmail_ReturnsTrue()
    {
        // Arrange
        using var context = CreateContext();
        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);

        // Act
        var result = await service.IsEmailAvailableAsync("new@example.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsEmailAvailableAsync_WithTakenEmail_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Id = 1, Username = "user", Email = "existing@example.com", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);

        // Act
        var result = await service.IsEmailAvailableAsync("existing@example.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidUsername_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);
        var loginDto = new LoginDto { Username = "nonexistent", Password = "password" };

        // Act
        var result = await service.AuthenticateAsync(loginDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User
        {
            Id = 1,
            Username = "inactiveuser",
            Email = "inactive@example.com",
            IsActive = false,
            PasswordHash = "hash"
        });
        await context.SaveChangesAsync();

        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);
        var loginDto = new LoginDto { Username = "inactiveuser", Password = "password" };

        // Act
        var result = await service.AuthenticateAsync(loginDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_SetsUserRoleToUser()
    {
        // Arrange
        using var context = CreateContext();
        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@example.com",
            Password = "password123"
        };

        // Act
        var result = await service.RegisterAsync(registerDto);

        // Assert
        Assert.Equal(UserRole.User, result.Role);
    }

    [Fact]
    public async Task RegisterAsync_SetsIsActiveToTrue()
    {
        // Arrange
        using var context = CreateContext();
        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@example.com",
            Password = "password123"
        };

        // Act
        var result = await service.RegisterAsync(registerDto);

        // Assert
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task RegisterAsync_HashesPassword()
    {
        // Arrange
        using var context = CreateContext();
        var service = new UserService(context, _mapper, _loggerMock.Object, _configurationMock.Object);
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@example.com",
            Password = "password123"
        };

        // Act
        await service.RegisterAsync(registerDto);

        // Assert
        var user = await context.Users.FirstAsync();
        Assert.NotEqual("password123", user.PasswordHash);
        Assert.NotEmpty(user.PasswordHash);
    }
}
