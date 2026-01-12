using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SampleDotNet6App.Data;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SampleDotNet6App.Services;

/// <summary>
/// User service implementation
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserService(ApplicationDbContext context, IMapper mapper, ILogger<UserService> logger, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<AuthResponseDto?> AuthenticateAsync(LoginDto loginDto)
    {
        _logger.LogInformation("Authenticating user: {Username}", loginDto.Username);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => (u.Username == loginDto.Username || u.Email == loginDto.Username) && u.IsActive);

        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Authentication failed for user: {Username}", loginDto.Username);
            return null;
        }

        var token = GenerateJwtToken(user);
        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResponseDto
        {
            Token = token,
            User = userDto
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
    {
        _logger.LogInformation("Registering new user: {Username}", registerDto.Username);

        var user = _mapper.Map<User>(registerDto);
        user.PasswordHash = HashPassword(registerDto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        _logger.LogInformation("Getting all users");

        var users = await _context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        _logger.LogInformation("Getting user with ID: {UserId}", id);

        var user = await _context.Users.FindAsync(id);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        _logger.LogInformation("Getting user with username: {Username}", username);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<bool> IsUsernameAvailableAsync(string username)
    {
        return !await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> IsEmailAvailableAsync(string email)
    {
        return !await _context.Users.AnyAsync(u => u.Email == email);
    }

    private string HashPassword(string password)
    {
        var dummyUser = new User();
        return _passwordHasher.HashPassword(dummyUser, password);
    }

    private bool VerifyPassword(string password, string hash)
    {
        var dummyUser = new User();
        var result = _passwordHasher.VerifyHashedPassword(dummyUser, hash, password);
        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "your-256-bit-secret-key-here-make-it-long-enough");
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
