using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SampleDotNet6App.Controllers;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;
using System.Security.Claims;
using Xunit;

namespace SampleDotNet6App.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILogger<UsersController>> _mockLogger;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<UsersController>>();
        _controller = new UsersController(_mockUserService.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var controller = new UsersController(_mockUserService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password" };
        var authResponse = new AuthResponseDto
        {
            Token = "test-token",
            User = new UserDto { Id = 1, Username = "testuser" }
        };
        _mockUserService.Setup(s => s.AuthenticateAsync(loginDto))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal("test-token", returnValue.Token);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "wrong" };
        _mockUserService.Setup(s => s.AuthenticateAsync(loginDto))
            .ReturnsAsync((AuthResponseDto?)null);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var loginDto = new LoginDto();
        _controller.ModelState.AddModelError("Username", "Required");

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "newuser@test.com",
            Password = "password",
            FirstName = "New",
            LastName = "User"
        };
        var userDto = new UserDto { Id = 1, Username = "newuser", Email = "newuser@test.com" };
        _mockUserService.Setup(s => s.IsUsernameAvailableAsync("newuser"))
            .ReturnsAsync(true);
        _mockUserService.Setup(s => s.IsEmailAvailableAsync("newuser@test.com"))
            .ReturnsAsync(true);
        _mockUserService.Setup(s => s.RegisterAsync(registerDto))
            .ReturnsAsync(userDto);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<UserDto>(createdResult.Value);
        Assert.Equal("newuser", returnValue.Username);
    }

    [Fact]
    public async Task Register_WithTakenUsername_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto { Username = "existinguser", Email = "test@test.com" };
        _mockUserService.Setup(s => s.IsUsernameAvailableAsync("existinguser"))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Username is already taken", badRequestResult.Value);
    }

    [Fact]
    public async Task Register_WithTakenEmail_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto { Username = "newuser", Email = "existing@test.com" };
        _mockUserService.Setup(s => s.IsUsernameAvailableAsync("newuser"))
            .ReturnsAsync(true);
        _mockUserService.Setup(s => s.IsEmailAvailableAsync("existing@test.com"))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Email is already registered", badRequestResult.Value);
    }

    [Fact]
    public async Task Register_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto();
        _controller.ModelState.AddModelError("Username", "Required");

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetUsers_ReturnsOkResult()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new UserDto { Id = 1, Username = "user1" },
            new UserDto { Id = 2, Username = "user2" }
        };
        _mockUserService.Setup(s => s.GetAllUsersAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _controller.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetUser_WithValidId_ReturnsUser()
    {
        // Arrange
        var user = new UserDto { Id = 1, Username = "testuser" };
        _mockUserService.Setup(s => s.GetUserByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetUser(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task GetUser_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetUserByIdAsync(999))
            .ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.GetUser(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CheckUsername_WithAvailableUsername_ReturnsAvailable()
    {
        // Arrange
        _mockUserService.Setup(s => s.IsUsernameAvailableAsync("newuser"))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CheckUsername("newuser");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task CheckUsername_WithTakenUsername_ReturnsUnavailable()
    {
        // Arrange
        _mockUserService.Setup(s => s.IsUsernameAvailableAsync("existinguser"))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CheckUsername("existinguser");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task CheckUsername_WithEmptyUsername_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.CheckUsername("");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CheckEmail_WithAvailableEmail_ReturnsAvailable()
    {
        // Arrange
        _mockUserService.Setup(s => s.IsEmailAvailableAsync("new@test.com"))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CheckEmail("new@test.com");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task CheckEmail_WithTakenEmail_ReturnsUnavailable()
    {
        // Arrange
        _mockUserService.Setup(s => s.IsEmailAvailableAsync("existing@test.com"))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CheckEmail("existing@test.com");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task CheckEmail_WithEmptyEmail_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.CheckEmail("");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
