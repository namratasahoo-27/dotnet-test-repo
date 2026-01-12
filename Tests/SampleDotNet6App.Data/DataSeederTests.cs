using Xunit;
using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SampleDotNet6App.Data.Tests;

public class DataSeederTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

    public DataSeederTests()
    {
        _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
    }

    private ApplicationDbContext CreateContext()
    {
        return new ApplicationDbContext(_dbOptions);
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsUsers()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var users = await context.Users.ToListAsync();
        Assert.NotEmpty(users);
        Assert.True(users.Count >= 3);
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsProducts()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var products = await context.Products.ToListAsync();
        Assert.NotEmpty(products);
        Assert.True(products.Count >= 5);
    }

    [Fact]
    public async Task SeedAsync_SeedsAdminUser()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var admin = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
        Assert.NotNull(admin);
        Assert.Equal(UserRole.Admin, admin.Role);
        Assert.Equal("admin@example.com", admin.Email);
    }

    [Fact]
    public async Task SeedAsync_SeedsRegularUser()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == "john.doe");
        Assert.NotNull(user);
        Assert.Equal(UserRole.User, user.Role);
    }

    [Fact]
    public async Task SeedAsync_SeedsManagerUser()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var manager = await context.Users.FirstOrDefaultAsync(u => u.Username == "jane.smith");
        Assert.NotNull(manager);
        Assert.Equal(UserRole.Manager, manager.Role);
    }

    [Fact]
    public async Task SeedAsync_SeedsProductsWithCorrectData()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var laptop = await context.Products.FirstOrDefaultAsync(p => p.Name == "Laptop Computer");
        Assert.NotNull(laptop);
        Assert.Equal("Electronics", laptop.Category);
        Assert.True(laptop.Price > 0);
        Assert.True(laptop.StockQuantity > 0);
        Assert.True(laptop.IsActive);
    }

    [Fact]
    public async Task SeedAsync_WithExistingUsers_DoesNotDuplicateUsers()
    {
        // Arrange
        using var context = CreateContext();
        await DataSeeder.SeedAsync(context);
        var initialUserCount = await context.Users.CountAsync();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var finalUserCount = await context.Users.CountAsync();
        Assert.Equal(initialUserCount, finalUserCount);
    }

    [Fact]
    public async Task SeedAsync_WithExistingProducts_DoesNotDuplicateProducts()
    {
        // Arrange
        using var context = CreateContext();
        await DataSeeder.SeedAsync(context);
        var initialProductCount = await context.Products.CountAsync();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var finalProductCount = await context.Products.CountAsync();
        Assert.Equal(initialProductCount, finalProductCount);
    }

    [Fact]
    public async Task SeedAsync_SeedsUsersWithHashedPasswords()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var users = await context.Users.ToListAsync();
        foreach (var user in users)
        {
            Assert.NotEmpty(user.PasswordHash);
            Assert.DoesNotContain("Admin123!", user.PasswordHash);
            Assert.DoesNotContain("User123!", user.PasswordHash);
            Assert.DoesNotContain("Manager123!", user.PasswordHash);
        }
    }

    [Fact]
    public async Task SeedAsync_SetsAllUsersAsActive()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var users = await context.Users.ToListAsync();
        Assert.All(users, user => Assert.True(user.IsActive));
    }

    [Fact]
    public async Task SeedAsync_SetsAllProductsAsActive()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var products = await context.Products.ToListAsync();
        Assert.All(products, product => Assert.True(product.IsActive));
    }

    [Fact]
    public async Task SeedAsync_SetsCreatedAtForUsers()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var users = await context.Users.ToListAsync();
        Assert.All(users, user => Assert.True(user.CreatedAt > System.DateTime.MinValue));
    }

    [Fact]
    public async Task SeedAsync_SetsCreatedAtAndUpdatedAtForProducts()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var products = await context.Products.ToListAsync();
        Assert.All(products, product =>
        {
            Assert.True(product.CreatedAt > System.DateTime.MinValue);
            Assert.True(product.UpdatedAt > System.DateTime.MinValue);
        });
    }

    [Fact]
    public async Task SeedAsync_SeedsDifferentProductCategories()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var categories = await context.Products.Select(p => p.Category).Distinct().ToListAsync();
        Assert.True(categories.Count >= 3);
        Assert.Contains("Electronics", categories);
        Assert.Contains("Furniture", categories);
    }

    [Fact]
    public async Task SeedAsync_EnsuresDatabaseIsCreated()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        Assert.True(await context.Database.CanConnectAsync());
    }
}
