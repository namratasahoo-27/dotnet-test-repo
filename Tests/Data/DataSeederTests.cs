using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using Xunit;

namespace SampleDotNet6App.Tests.Data;

public class DataSeederTests
{
    private DbContextOptions<ApplicationDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsUsers()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var userCount = await context.Users.CountAsync();
        Assert.True(userCount > 0);
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsProducts()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var productCount = await context.Products.CountAsync();
        Assert.True(productCount > 0);
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsAdminUser()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
        Assert.NotNull(adminUser);
        Assert.Equal(UserRole.Admin, adminUser.Role);
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsManagerUser()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var managerUser = await context.Users.FirstOrDefaultAsync(u => u.Role == UserRole.Manager);
        Assert.NotNull(managerUser);
    }

    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_SeedsRegularUser()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var regularUser = await context.Users.FirstOrDefaultAsync(u => u.Role == UserRole.User);
        Assert.NotNull(regularUser);
    }

    [Fact]
    public async Task SeedAsync_WithExistingUsers_DoesNotDuplicateUsers()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act - seed twice
        await DataSeeder.SeedAsync(context);
        var countAfterFirstSeed = await context.Users.CountAsync();
        await DataSeeder.SeedAsync(context);
        var countAfterSecondSeed = await context.Users.CountAsync();

        // Assert
        Assert.Equal(countAfterFirstSeed, countAfterSecondSeed);
    }

    [Fact]
    public async Task SeedAsync_WithExistingProducts_DoesNotDuplicateProducts()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act - seed twice
        await DataSeeder.SeedAsync(context);
        var countAfterFirstSeed = await context.Products.CountAsync();
        await DataSeeder.SeedAsync(context);
        var countAfterSecondSeed = await context.Products.CountAsync();

        // Assert
        Assert.Equal(countAfterFirstSeed, countAfterSecondSeed);
    }

    [Fact]
    public async Task SeedAsync_SeedsActiveUsers()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var users = await context.Users.ToListAsync();
        Assert.All(users, user => Assert.True(user.IsActive));
    }

    [Fact]
    public async Task SeedAsync_SeedsActiveProducts()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var products = await context.Products.ToListAsync();
        Assert.All(products, product => Assert.True(product.IsActive));
    }

    [Fact]
    public async Task SeedAsync_SeedsProductsWithValidData()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var products = await context.Products.ToListAsync();
        Assert.All(products, product =>
        {
            Assert.NotEmpty(product.Name);
            Assert.NotEmpty(product.Category);
            Assert.True(product.Price > 0);
            Assert.True(product.StockQuantity >= 0);
        });
    }

    [Fact]
    public async Task SeedAsync_SeedsUsersWithHashedPasswords()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var users = await context.Users.ToListAsync();
        Assert.All(users, user =>
        {
            Assert.NotEmpty(user.PasswordHash);
            Assert.NotEqual("password", user.PasswordHash);
        });
    }

    [Fact]
    public async Task SeedAsync_SeedsUsersWithUniqueUsernames()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var users = await context.Users.ToListAsync();
        var usernames = users.Select(u => u.Username).ToList();
        Assert.Equal(usernames.Count, usernames.Distinct().Count());
    }

    [Fact]
    public async Task SeedAsync_SeedsUsersWithUniqueEmails()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var users = await context.Users.ToListAsync();
        var emails = users.Select(u => u.Email).ToList();
        Assert.Equal(emails.Count, emails.Distinct().Count());
    }
}
