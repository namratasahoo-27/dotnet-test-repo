using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Models;
using System.Security.Cryptography;
using System.Text;

namespace SampleDotNet6App.Data;

/// <summary>
/// Seeds the database with initial data
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Seeds the database with sample data
    /// </summary>
    /// <param name="context">Database context</param>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed Users
        if (!await context.Users.AnyAsync())
        {
            var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    FirstName = "System",
                    LastName = "Administrator",
                    PasswordHash = HashPassword("Admin123!"),
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Username = "john.doe",
                    Email = "john.doe@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    PasswordHash = HashPassword("User123!"),
                    Role = UserRole.User,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Username = "jane.smith",
                    Email = "jane.smith@example.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    PasswordHash = HashPassword("Manager123!"),
                    Role = UserRole.Manager,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        // Seed Products
        if (!await context.Products.AnyAsync())
        {
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Laptop Computer",
                    Description = "High-performance laptop for professional use",
                    Price = 1299.99m,
                    Category = "Electronics",
                    StockQuantity = 50,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Product
                {
                    Name = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse with precision tracking",
                    Price = 49.99m,
                    Category = "Electronics",
                    StockQuantity = 200,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Product
                {
                    Name = "Office Chair",
                    Description = "Comfortable ergonomic office chair",
                    Price = 299.99m,
                    Category = "Furniture",
                    StockQuantity = 25,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Product
                {
                    Name = "Desk Lamp",
                    Description = "LED desk lamp with adjustable brightness",
                    Price = 79.99m,
                    Category = "Lighting",
                    StockQuantity = 100,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Product
                {
                    Name = "Coffee Mug",
                    Description = "Ceramic coffee mug with company logo",
                    Price = 12.99m,
                    Category = "Accessories",
                    StockQuantity = 500,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
