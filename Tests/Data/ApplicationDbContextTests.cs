using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using Xunit;

namespace SampleDotNet6App.Tests.Data;

public class ApplicationDbContextTests
{
    private DbContextOptions<ApplicationDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidOptions_CreatesInstance()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void Products_DbSet_IsNotNull()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.Products);
    }

    [Fact]
    public void Users_DbSet_IsNotNull()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.Users);
    }

    [Fact]
    public async Task AddProduct_SavesProduct()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Category = "Test",
            StockQuantity = 10,
            IsActive = true
        };

        // Act
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, await context.Products.CountAsync());
    }

    [Fact]
    public async Task AddUser_SavesUser()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash",
            Role = UserRole.User,
            IsActive = true
        };

        // Act
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, await context.Users.CountAsync());
    }

    [Fact]
    public async Task FindProduct_ById_ReturnsProduct()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Category = "Test",
            StockQuantity = 10
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Act
        var foundProduct = await context.Products.FindAsync(product.Id);

        // Assert
        Assert.NotNull(foundProduct);
        Assert.Equal("Test Product", foundProduct.Name);
    }

    [Fact]
    public async Task FindUser_ById_ReturnsUser()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var foundUser = await context.Users.FindAsync(user.Id);

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal("testuser", foundUser.Username);
    }

    [Fact]
    public async Task RemoveProduct_DeletesProduct()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var product = new Product
        {
            Name = "Test Product",
            Price = 99.99m,
            Category = "Test"
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Act
        context.Products.Remove(product);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(0, await context.Products.CountAsync());
    }

    [Fact]
    public async Task UpdateProduct_UpdatesProduct()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var product = new Product
        {
            Name = "Original Name",
            Price = 99.99m,
            Category = "Test"
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Act
        product.Name = "Updated Name";
        await context.SaveChangesAsync();

        // Assert
        var updatedProduct = await context.Products.FindAsync(product.Id);
        Assert.Equal("Updated Name", updatedProduct!.Name);
    }

    [Fact]
    public async Task AddMultipleProducts_SavesAllProducts()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var products = new List<Product>
        {
            new Product { Name = "Product 1", Price = 10m, Category = "Test" },
            new Product { Name = "Product 2", Price = 20m, Category = "Test" },
            new Product { Name = "Product 3", Price = 30m, Category = "Test" }
        };

        // Act
        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(3, await context.Products.CountAsync());
    }

    [Fact]
    public async Task QueryProducts_ByCategory_ReturnsFilteredProducts()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        context.Products.AddRange(
            new Product { Name = "Product 1", Price = 10m, Category = "Electronics" },
            new Product { Name = "Product 2", Price = 20m, Category = "Books" },
            new Product { Name = "Product 3", Price = 30m, Category = "Electronics" }
        );
        await context.SaveChangesAsync();

        // Act
        var electronics = await context.Products
            .Where(p => p.Category == "Electronics")
            .ToListAsync();

        // Assert
        Assert.Equal(2, electronics.Count);
    }
}
