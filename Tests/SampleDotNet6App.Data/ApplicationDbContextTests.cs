using Xunit;
using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SampleDotNet6App.Data.Tests;

public class ApplicationDbContextTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

    public ApplicationDbContextTests()
    {
        _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private ApplicationDbContext CreateContext()
    {
        return new ApplicationDbContext(_dbOptions);
    }

    [Fact]
    public void Constructor_WithValidOptions_CreatesInstance()
    {
        // Arrange & Act
        using var context = CreateContext();

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void Products_DbSet_IsNotNull()
    {
        // Arrange
        using var context = CreateContext();

        // Assert
        Assert.NotNull(context.Products);
    }

    [Fact]
    public void Users_DbSet_IsNotNull()
    {
        // Arrange
        using var context = CreateContext();

        // Assert
        Assert.NotNull(context.Users);
    }

    [Fact]
    public async Task CanAddAndRetrieveProduct()
    {
        // Arrange
        using var context = CreateContext();
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Category = "Test",
            StockQuantity = 10,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Assert
        var retrievedProduct = await context.Products.FirstOrDefaultAsync();
        Assert.NotNull(retrievedProduct);
        Assert.Equal("Test Product", retrievedProduct.Name);
    }

    [Fact]
    public async Task CanAddAndRetrieveUser()
    {
        // Arrange
        using var context = CreateContext();
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash123",
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Assert
        var retrievedUser = await context.Users.FirstOrDefaultAsync();
        Assert.NotNull(retrievedUser);
        Assert.Equal("testuser", retrievedUser.Username);
    }

    [Fact]
    public async Task Product_NameIsRequired()
    {
        // Arrange
        using var context = CreateContext();
        var product = new Product
        {
            Name = null!,
            Price = 10m,
            Category = "Test",
            IsActive = true
        };

        // Act & Assert
        context.Products.Add(product);
        await Assert.ThrowsAnyAsync<Exception>(async () => await context.SaveChangesAsync());
    }

    [Fact]
    public async Task Product_CategoryIsRequired()
    {
        // Arrange
        using var context = CreateContext();
        var product = new Product
        {
            Name = "Test",
            Price = 10m,
            Category = null!,
            IsActive = true
        };

        // Act & Assert
        context.Products.Add(product);
        await Assert.ThrowsAnyAsync<Exception>(async () => await context.SaveChangesAsync());
    }

    [Fact]
    public async Task User_UsernameIsRequired()
    {
        // Arrange
        using var context = CreateContext();
        var user = new User
        {
            Username = null!,
            Email = "test@example.com",
            PasswordHash = "hash"
        };

        // Act & Assert
        context.Users.Add(user);
        await Assert.ThrowsAnyAsync<Exception>(async () => await context.SaveChangesAsync());
    }

    [Fact]
    public async Task User_EmailIsRequired()
    {
        // Arrange
        using var context = CreateContext();
        var user = new User
        {
            Username = "testuser",
            Email = null!,
            PasswordHash = "hash"
        };

        // Act & Assert
        context.Users.Add(user);
        await Assert.ThrowsAnyAsync<Exception>(async () => await context.SaveChangesAsync());
    }

    [Fact]
    public async Task CanUpdateProduct()
    {
        // Arrange
        using var context = CreateContext();
        var product = new Product
        {
            Name = "Original Name",
            Price = 10m,
            Category = "Test",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Act
        product.Name = "Updated Name";
        await context.SaveChangesAsync();

        // Assert
        var updatedProduct = await context.Products.FirstOrDefaultAsync();
        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated Name", updatedProduct.Name);
    }

    [Fact]
    public async Task CanDeleteProduct()
    {
        // Arrange
        using var context = CreateContext();
        var product = new Product
        {
            Name = "Test",
            Price = 10m,
            Category = "Test",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Act
        context.Products.Remove(product);
        await context.SaveChangesAsync();

        // Assert
        var count = await context.Products.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task CanDeleteUser()
    {
        // Arrange
        using var context = CreateContext();
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        context.Users.Remove(user);
        await context.SaveChangesAsync();

        // Assert
        var count = await context.Users.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task CanQueryProductsByCategory()
    {
        // Arrange
        using var context = CreateContext();
        context.Products.AddRange(
            new Product { Name = "P1", Price = 10m, Category = "Electronics", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Product { Name = "P2", Price = 20m, Category = "Furniture", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        // Act
        var electronics = await context.Products.Where(p => p.Category == "Electronics").ToListAsync();

        // Assert
        Assert.Single(electronics);
        Assert.Equal("P1", electronics[0].Name);
    }

    [Fact]
    public async Task CanQueryUsersByRole()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.AddRange(
            new User { Username = "admin", Email = "admin@example.com", PasswordHash = "hash", Role = UserRole.Admin, IsActive = true, CreatedAt = DateTime.UtcNow },
            new User { Username = "user", Email = "user@example.com", PasswordHash = "hash", Role = UserRole.User, IsActive = true, CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        // Act
        var admins = await context.Users.Where(u => u.Role == UserRole.Admin).ToListAsync();

        // Assert
        Assert.Single(admins);
        Assert.Equal("admin", admins[0].Username);
    }

    [Fact]
    public async Task Product_PriceStoredCorrectly()
    {
        // Arrange
        using var context = CreateContext();
        var product = new Product
        {
            Name = "Test",
            Price = 123.45m,
            Category = "Test",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Assert
        var savedProduct = await context.Products.FirstOrDefaultAsync();
        Assert.NotNull(savedProduct);
        Assert.Equal(123.45m, savedProduct.Price);
    }
}
