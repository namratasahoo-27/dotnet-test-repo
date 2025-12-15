using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Controllers;

/// <summary>
/// Users API controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>Authentication token and user information</returns>
    /// <response code="200">Login successful</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        _logger.LogInformation("POST /api/users/login called for user: {Username}", loginDto.Username);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var authResponse = await _userService.AuthenticateAsync(loginDto);
        
        if (authResponse == null)
        {
            return Unauthorized("Invalid username or password");
        }

        return Ok(authResponse);
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="registerDto">User registration data</param>
    /// <returns>Created user information</returns>
    /// <response code="201">User created successfully</response>
    /// <response code="400">Invalid input data or username/email already exists</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        _logger.LogInformation("POST /api/users/register called for user: {Username}", registerDto.Username);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if username is available
        if (!await _userService.IsUsernameAvailableAsync(registerDto.Username))
        {
            return BadRequest("Username is already taken");
        }

        // Check if email is available
        if (!await _userService.IsEmailAvailableAsync(registerDto.Email))
        {
            return BadRequest("Email is already registered");
        }

        var user = await _userService.RegisterAsync(registerDto);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    /// <summary>
    /// Gets all users (Admin only)
    /// </summary>
    /// <returns>List of users</returns>
    /// <response code="200">Returns the list of users</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        _logger.LogInformation("GET /api/users called");
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Gets a specific user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    /// <response code="200">Returns the user</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        _logger.LogInformation("GET /api/users/{UserId} called", id);
        var user = await _userService.GetUserByIdAsync(id);
        
        if (user == null)
        {
            return NotFound($"User with ID {id} not found");
        }

        return Ok(user);
    }

    /// <summary>
    /// Gets the current user's profile
    /// </summary>
    /// <returns>Current user details</returns>
    /// <response code="200">Returns the current user</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }

        _logger.LogInformation("GET /api/users/profile called for user: {Username}", username);
        var user = await _userService.GetUserByUsernameAsync(username);
        
        if (user == null)
        {
            return NotFound("User profile not found");
        }

        return Ok(user);
    }

    /// <summary>
    /// Checks if a username is available
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <returns>Availability status</returns>
    /// <response code="200">Returns availability status</response>
    /// <response code="400">Invalid username</response>
    [HttpGet("check-username")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CheckUsername([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest("Username cannot be empty");
        }

        _logger.LogInformation("GET /api/users/check-username called for: {Username}", username);
        var isAvailable = await _userService.IsUsernameAvailableAsync(username);
        return Ok(new { username, available = isAvailable });
    }

    /// <summary>
    /// Checks if an email is available
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <returns>Availability status</returns>
    /// <response code="200">Returns availability status</response>
    /// <response code="400">Invalid email</response>
    [HttpGet("check-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CheckEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email cannot be empty");
        }

        _logger.LogInformation("GET /api/users/check-email called for: {Email}", email);
        var isAvailable = await _userService.IsEmailAvailableAsync(email);
        return Ok(new { email, available = isAvailable });
    }
}
