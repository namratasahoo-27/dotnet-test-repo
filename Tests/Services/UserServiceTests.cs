using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SampleDotNet6App.Data;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;
using Xunit;

namespace SampleDotNet6App.Tests.Services;

public class UserServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly UserService _service;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        _mockLogger = new Mock<ILogger<UserService>>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("your-256-bit-secret-key-here-make-it-long-enough");

        _service = new UserService(_context, _mapper, _mockLogger.Object, _mockConfiguration.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Assert
        Assert.NotNull(_service);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = HashPassword("password123"),
            IsActive = true,
            Role = UserRole.User
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginDto = new LoginDto
        {
            Username = "testuser",
            Password = "password123"
        };

        // Act
        var result = await _service.AuthenticateAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        Assert.Equal(user.Username, result.User.Username);
    }

    [Fact]
    public async Task AuthenticateAsync_WithEmail_ReturnsAuthResponse()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = HashPassword("password123"),
            IsActive = true,
            Role = UserRole.User
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginDto = new LoginDto
        {
            Username = "test@test.com",
            Password = "password123"
        };

        // Act
        var result = await _service.AuthenticateAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            PasswordHash = HashPassword("password123"),
            IsActive = true
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginDto = new LoginDto
        {
            Username = "testuser",
            Password = "wrongpassword"
        };

        // Act
        var result = await _service.AuthenticateAsync(loginDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            PasswordHash = HashPassword("password123"),
            IsActive = false
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginDto = new LoginDto
        {
            Username = "testuser",
            Password = "password123"
        };

        // Act
        var result = await _service.AuthenticateAsync(loginDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithNonExistentUser_ReturnsNull()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "nonexistent",
            Password = "password"
        };

        // Act
        var result = await _service.AuthenticateAsync(loginDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_CreatesUser()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@test.com",
            FirstName = "New",
            LastName = "User",
            Password = "password123"
        };

        // Act
        var result = await _service.RegisterAsync(registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(registerDto.Username, result.Username);
        Assert.Equal(1, await _context.Users.CountAsync());
    }

    [Fact]
    public async Task RegisterAsync_HashesPassword()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@test.com",
            Password = "password123"
        };

        // Act
        await _service.RegisterAsync(registerDto);

        // Assert
        var user = await _context.Users.FirstAsync();
        Assert.NotEqual("password123", user.PasswordHash);
        Assert.NotEmpty(user.PasswordHash);
    }

    [Fact]
    public async Task RegisterAsync_SetsUserRole()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@test.com",
            Password = "password123"
        };

        // Act
        var result = await _service.RegisterAsync(registerDto);

        // Assert
        Assert.Equal(UserRole.User, result.Role);
    }

    [Fact]
    public async Task RegisterAsync_SetsIsActiveToTrue()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@test.com",
            Password = "password123"
        };

        // Act
        var result = await _service.RegisterAsync(registerDto);

        // Assert
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllActiveUsers()
    {
        // Arrange
        _context.Users.AddRange(
            new User { Username = "user1", Email = "user1@test.com", IsActive = true },
            new User { Username = "user2", Email = "user2@test.com", IsActive = true },
            new User { Username = "user3", Email = "user3@test.com", IsActive = false }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllUsersAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllUsersAsync_OrdersByUsername()
    {
        // Arrange
        _context.Users.AddRange(
            new User { Username = "zebra", Email = "z@test.com", IsActive = true },
            new User { Username = "alpha", Email = "a@test.com", IsActive = true },
            new User { Username = "beta", Email = "b@test.com", IsActive = true }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllUsersAsync();

        // Assert
        var usernames = result.Select(u => u.Username).ToList();
        Assert.Equal("alpha", usernames[0]);
        Assert.Equal("beta", usernames[1]);
        Assert.Equal("zebra", usernames[2]);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@test.com" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetUserByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _service.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithValidUsername_ReturnsUser()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@test.com", IsActive = true };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetUserByUsernameAsync("testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@test.com", IsActive = false };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetUserByUsernameAsync("testuser");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithInvalidUsername_ReturnsNull()
    {
        // Act
        var result = await _service.GetUserByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task IsUsernameAvailableAsync_WithAvailableUsername_ReturnsTrue()
    {
        // Act
        var result = await _service.IsUsernameAvailableAsync("newuser");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsUsernameAvailableAsync_WithTakenUsername_ReturnsFalse()
    {
        // Arrange
        _context.Users.Add(new User { Username = "existinguser", Email = "test@test.com" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.IsUsernameAvailableAsync("existinguser");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsEmailAvailableAsync_WithAvailableEmail_ReturnsTrue()
    {
        // Act
        var result = await _service.IsEmailAvailableAsync("new@test.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsEmailAvailableAsync_WithTakenEmail_ReturnsFalse()
    {
        // Arrange
        _context.Users.Add(new User { Username = "user", Email = "existing@test.com" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.IsEmailAvailableAsync("existing@test.com");

        // Assert
        Assert.False(result);
    }

    private string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
