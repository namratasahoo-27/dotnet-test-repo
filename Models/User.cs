namespace SampleDotNet6App.Models;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User
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
    /// The hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// The user's role
    /// </summary>
    public UserRole Role { get; set; } = UserRole.User;

    /// <summary>
    /// When the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Whether the user is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// User roles in the system
/// </summary>
public enum UserRole
{
    User = 1,
    Admin = 2,
    Manager = 3
}
