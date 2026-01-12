using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Services;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Mappings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleDotNet6App.Services.Tests;

public class ProductServiceTests
{
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

    public ProductServiceTests()
    {
        _loggerMock = new Mock<ILogger<ProductService>>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
    }

    private ApplicationDbContext CreateContext()
    {
        return new ApplicationDbContext(_dbOptions);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var service = new ProductService(context, _mapper, _loggerMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        // Arrange
        using var context = CreateContext();
        context.Products.AddRange(
            new Product { Id = 1, Name = "Product 1", Price = 10m, Category = "Cat1", IsActive = true },
            new Product { Id = 2, Name = "Product 2", Price = 20m, Category = "Cat2", IsActive = true }
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllProductsAsync_WithCategoryFilter_ReturnsFilteredProducts()
    {
        // Arrange
        using var context = CreateContext();
        context.Products.AddRange(
            new Product { Id = 1, Name = "Product 1", Price = 10m, Category = "Electronics", IsActive = true },
            new Product { Id = 2, Name = "Product 2", Price = 20m, Category = "Furniture", IsActive = true }
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetAllProductsAsync("Electronics");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Electronics", result.First().Category);
    }

    [Fact]
    public async Task GetAllProductsAsync_WithIsActiveFilter_ReturnsFilteredProducts()
    {
        // Arrange
        using var context = CreateContext();
        context.Products.AddRange(
            new Product { Id = 1, Name = "Product 1", Price = 10m, Category = "Cat1", IsActive = true },
            new Product { Id = 2, Name = "Product 2", Price = 20m, Category = "Cat2", IsActive = false }
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetAllProductsAsync(null, true);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.True(result.First().IsActive);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "Test Product", Price = 10m, Category = "Test", IsActive = true });
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetProductByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Product", result.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var service = new ProductService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetProductByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProductAsync_CreatesAndReturnsProduct()
    {
        // Arrange
        using var context = CreateContext();
        var service = new ProductService(context, _mapper, _loggerMock.Object);
        var createDto = new CreateProductDto
        {
            Name = "New Product",
            Description = "New Description",
            Price = 50m,
            Category = "New Category",
            StockQuantity = 10
        };

        // Act
        var result = await service.CreateProductAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        Assert.Equal(50m, result.Price);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task UpdateProductAsync_WithValidId_UpdatesAndReturnsProduct()
    {
        // Arrange
        using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "Old Name", Price = 10m, Category = "Old", IsActive = true });
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper, _loggerMock.Object);
        var updateDto = new UpdateProductDto { Name = "New Name", Price = 20m };

        // Act
        var result = await service.UpdateProductAsync(1, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);
        Assert.Equal(20m, result.Price);
    }

    [Fact]
    public async Task UpdateProductAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var service = new ProductService(context, _mapper, _loggerMock.Object);
        var updateDto = new UpdateProductDto { Name = "New Name" };

        // Act
        var result = await service.UpdateProductAsync(999, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteProductAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "Test", Price = 10m, Category = "Test", IsActive = true });
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.DeleteProductAsync(1);

        // Assert
        Assert.True(result);
        var deletedProduct = await context.Products.FindAsync(1);
        Assert.Null(deletedProduct);
    }

    [Fact]
    public async Task DeleteProductAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var service = new ProductService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.DeleteProductAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchProductsAsync_ReturnsMatchingProducts()
    {
        // Arrange
        using var context = CreateContext();
        context.Products.AddRange(
            new Product { Id = 1, Name = "Laptop Computer", Description = "High-end laptop", Price = 1000m, Category = "Electronics", IsActive = true },
            new Product { Id = 2, Name = "Desktop Computer", Description = "Powerful desktop", Price = 1500m, Category = "Electronics", IsActive = true },
            new Product { Id = 3, Name = "Office Chair", Description = "Comfortable chair", Price = 200m, Category = "Furniture", IsActive = true }
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.SearchProductsAsync("Computer");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchProductsAsync_SearchesInDescription()
    {
        // Arrange
        using var context = CreateContext();
        context.Products.AddRange(
            new Product { Id = 1, Name = "Product A", Description = "Contains laptop keyword", Price = 100m, Category = "Test", IsActive = true },
            new Product { Id = 2, Name = "Product B", Description = "Different description", Price = 200m, Category = "Test", IsActive = true }
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.SearchProductsAsync("laptop");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchProductsAsync_SearchesInCategory()
    {
        // Arrange
        using var context = CreateContext();
        context.Products.AddRange(
            new Product { Id = 1, Name = "Product A", Description = "Desc", Price = 100m, Category = "Electronics", IsActive = true },
            new Product { Id = 2, Name = "Product B", Description = "Desc", Price = 200m, Category = "Furniture", IsActive = true }
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.SearchProductsAsync("Electronics");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchProductsAsync_OnlyReturnsActiveProducts()
    {
        // Arrange
        using var context = CreateContext();
        context.Products.AddRange(
            new Product { Id = 1, Name = "Active Product", Description = "Test", Price = 100m, Category = "Test", IsActive = true },
            new Product { Id = 2, Name = "Inactive Product", Description = "Test", Price = 200m, Category = "Test", IsActive = false }
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper, _loggerMock.Object);

        // Act
        var result = await service.SearchProductsAsync("Product");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.True(result.First().IsActive);
    }
}
