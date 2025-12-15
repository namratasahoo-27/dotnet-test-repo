using SampleDotNet6App.Models;

namespace SampleDotNet6App.DTOs;

/// <summary>
/// User data transfer object for API responses
/// </summary>
public class UserDto
{
    /// <summary>
    /// The unique identifier for the user
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The user's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The user's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The user's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The user's role
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Whether the user is active
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Login request DTO
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Username or email
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Register user request DTO
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// The username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The user's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The user's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The user's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The password
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Authentication response DTO
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// User information
    /// </summary>
    public UserDto User { get; set; } = new();
}
