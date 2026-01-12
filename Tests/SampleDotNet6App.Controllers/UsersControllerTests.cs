using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SampleDotNet6App.Controllers;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Services;
using SampleDotNet6App.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SampleDotNet6App.Controllers.Tests;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ILogger<UsersController>> _loggerMock;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<UsersController>>();
        _controller = new UsersController(_userServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var controller = new UsersController(_userServiceMock.Object, _loggerMock.Object);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password123" };
        var authResponse = new AuthResponseDto
        {
            Token = "test-token",
            User = new UserDto { Id = 1, Username = "testuser" }
        };
        _userServiceMock.Setup(x => x.AuthenticateAsync(loginDto))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAuth = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal("test-token", returnedAuth.Token);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "invalid", Password = "wrong" };
        _userServiceMock.Setup(x => x.AuthenticateAsync(loginDto))
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
        _controller.ModelState.AddModelError("Username", "Username is required");

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
            Email = "newuser@example.com",
            Password = "password123",
            FirstName = "New",
            LastName = "User"
        };
        var userDto = new UserDto { Id = 1, Username = "newuser", Email = "newuser@example.com" };
        _userServiceMock.Setup(x => x.IsUsernameAvailableAsync("newuser")).ReturnsAsync(true);
        _userServiceMock.Setup(x => x.IsEmailAvailableAsync("newuser@example.com")).ReturnsAsync(true);
        _userServiceMock.Setup(x => x.RegisterAsync(registerDto)).ReturnsAsync(userDto);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedUser = Assert.IsType<UserDto>(createdResult.Value);
        Assert.Equal("newuser", returnedUser.Username);
    }

    [Fact]
    public async Task Register_WithTakenUsername_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto { Username = "existinguser", Email = "test@example.com" };
        _userServiceMock.Setup(x => x.IsUsernameAvailableAsync("existinguser")).ReturnsAsync(false);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Register_WithTakenEmail_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto { Username = "newuser", Email = "existing@example.com" };
        _userServiceMock.Setup(x => x.IsUsernameAvailableAsync("newuser")).ReturnsAsync(true);
        _userServiceMock.Setup(x => x.IsEmailAvailableAsync("existing@example.com")).ReturnsAsync(false);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Register_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto();
        _controller.ModelState.AddModelError("Email", "Email is required");

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
        _userServiceMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
        Assert.Equal(2, ((List<UserDto>)returnedUsers).Count);
    }

    [Fact]
    public async Task GetUser_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var user = new UserDto { Id = 1, Username = "testuser" };
        _userServiceMock.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUser(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(1, returnedUser.Id);
    }

    [Fact]
    public async Task GetUser_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetUserByIdAsync(999)).ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.GetUser(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetProfile_WithAuthenticatedUser_ReturnsOkResult()
    {
        // Arrange
        var user = new UserDto { Id = 1, Username = "testuser" };
        _userServiceMock.Setup(x => x.GetUserByUsernameAsync("testuser")).ReturnsAsync(user);

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal("testuser", returnedUser.Username);
    }

    [Fact]
    public async Task GetProfile_WithNoAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _controller.GetProfile();

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetProfile_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetUserByUsernameAsync("testuser")).ReturnsAsync((UserDto?)null);

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.GetProfile();

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CheckUsername_WithAvailableUsername_ReturnsOkWithTrue()
    {
        // Arrange
        _userServiceMock.Setup(x => x.IsUsernameAvailableAsync("newuser")).ReturnsAsync(true);

        // Act
        var result = await _controller.CheckUsername("newuser");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task CheckUsername_WithTakenUsername_ReturnsOkWithFalse()
    {
        // Arrange
        _userServiceMock.Setup(x => x.IsUsernameAvailableAsync("existinguser")).ReturnsAsync(false);

        // Act
        var result = await _controller.CheckUsername("existinguser");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task CheckUsername_WithEmptyUsername_ReturnsBadRequest()
    {
        // Arrange & Act
        var result = await _controller.CheckUsername("");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CheckUsername_WithNullUsername_ReturnsBadRequest()
    {
        // Arrange & Act
        var result = await _controller.CheckUsername(null!);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CheckEmail_WithAvailableEmail_ReturnsOkWithTrue()
    {
        // Arrange
        _userServiceMock.Setup(x => x.IsEmailAvailableAsync("new@example.com")).ReturnsAsync(true);

        // Act
        var result = await _controller.CheckEmail("new@example.com");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task CheckEmail_WithTakenEmail_ReturnsOkWithFalse()
    {
        // Arrange
        _userServiceMock.Setup(x => x.IsEmailAvailableAsync("existing@example.com")).ReturnsAsync(false);

        // Act
        var result = await _controller.CheckEmail("existing@example.com");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task CheckEmail_WithEmptyEmail_ReturnsBadRequest()
    {
        // Arrange & Act
        var result = await _controller.CheckEmail("");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CheckEmail_WithNullEmail_ReturnsBadRequest()
    {
        // Arrange & Act
        var result = await _controller.CheckEmail(null!);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
