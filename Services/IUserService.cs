using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.Services;

/// <summary>
/// User service interface
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Authenticates a user
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>Authentication response if successful, null otherwise</returns>
    Task<AuthResponseDto?> AuthenticateAsync(LoginDto loginDto);

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="registerDto">Registration data</param>
    /// <returns>Registered user data</returns>
    Task<UserDto> RegisterAsync(RegisterDto registerDto);

    /// <summary>
    /// Gets all users
    /// </summary>
    /// <returns>List of users</returns>
    Task<IEnumerable<UserDto>> GetAllUsersAsync();

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User if found, null otherwise</returns>
    Task<UserDto?> GetUserByIdAsync(int id);

    /// <summary>
    /// Gets a user by username
    /// </summary>
    /// <param name="username">Username</param>
    /// <returns>User if found, null otherwise</returns>
    Task<UserDto?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Checks if a username is available
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <returns>True if available, false otherwise</returns>
    Task<bool> IsUsernameAvailableAsync(string username);

    /// <summary>
    /// Checks if an email is available
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <returns>True if available, false otherwise</returns>
    Task<bool> IsEmailAvailableAsync(string email);
}
