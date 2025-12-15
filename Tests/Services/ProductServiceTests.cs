using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SampleDotNet6App.Data;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;
using Xunit;

namespace SampleDotNet6App.Tests.Services;

public class ProductServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        _mockLogger = new Mock<ILogger<ProductService>>();
        _service = new ProductService(_context, _mapper, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Assert
        Assert.NotNull(_service);
    }

    [Fact]
    public async Task GetAllProductsAsync_WithNoFilters_ReturnsAllProducts()
    {
        // Arrange
        _context.Products.AddRange(
            new Product { Name = "Product 1", Category = "Electronics", Price = 10m, IsActive = true },
            new Product { Name = "Product 2", Category = "Books", Price = 20m, IsActive = true }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllProductsAsync_WithCategoryFilter_ReturnsFilteredProducts()
    {
        // Arrange
        _context.Products.AddRange(
            new Product { Name = "Product 1", Category = "Electronics", Price = 10m, IsActive = true },
            new Product { Name = "Product 2", Category = "Books", Price = 20m, IsActive = true }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllProductsAsync("Electronics");

        // Assert
        Assert.Single(result);
        Assert.Equal("Electronics", result.First().Category);
    }

    [Fact]
    public async Task GetAllProductsAsync_WithIsActiveFilter_ReturnsFilteredProducts()
    {
        // Arrange
        _context.Products.AddRange(
            new Product { Name = "Product 1", Category = "Test", Price = 10m, IsActive = true },
            new Product { Name = "Product 2", Category = "Test", Price = 20m, IsActive = false }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllProductsAsync(null, true);

        // Assert
        Assert.Single(result);
        Assert.True(result.First().IsActive);
    }

    [Fact]
    public async Task GetAllProductsAsync_WithBothFilters_ReturnsFilteredProducts()
    {
        // Arrange
        _context.Products.AddRange(
            new Product { Name = "Product 1", Category = "Electronics", Price = 10m, IsActive = true },
            new Product { Name = "Product 2", Category = "Electronics", Price = 20m, IsActive = false },
            new Product { Name = "Product 3", Category = "Books", Price = 30m, IsActive = true }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllProductsAsync("Electronics", true);

        // Assert
        Assert.Single(result);
        Assert.Equal("Electronics", result.First().Category);
        Assert.True(result.First().IsActive);
    }

    [Fact]
    public async Task GetAllProductsAsync_OrdersByName()
    {
        // Arrange
        _context.Products.AddRange(
            new Product { Name = "Zebra", Category = "Test", Price = 10m },
            new Product { Name = "Apple", Category = "Test", Price = 20m },
            new Product { Name = "Banana", Category = "Test", Price = 30m }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        var names = result.Select(p => p.Name).ToList();
        Assert.Equal("Apple", names[0]);
        Assert.Equal("Banana", names[1]);
        Assert.Equal("Zebra", names[2]);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        var product = new Product { Name = "Test Product", Category = "Test", Price = 99.99m };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetProductByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Name, result.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _service.GetProductByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProductAsync_WithValidData_CreatesProduct()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "New Product",
            Description = "Description",
            Price = 49.99m,
            Category = "Test",
            StockQuantity = 5
        };

        // Act
        var result = await _service.CreateProductAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(1, await _context.Products.CountAsync());
    }

    [Fact]
    public async Task CreateProductAsync_SetsIsActiveToTrue()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "New Product",
            Price = 49.99m,
            Category = "Test"
        };

        // Act
        var result = await _service.CreateProductAsync(createDto);

        // Assert
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task UpdateProductAsync_WithValidData_UpdatesProduct()
    {
        // Arrange
        var product = new Product { Name = "Original", Category = "Test", Price = 99.99m };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateProductDto
        {
            Name = "Updated",
            Price = 79.99m
        };

        // Act
        var result = await _service.UpdateProductAsync(product.Id, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Name);
        Assert.Equal(79.99m, result.Price);
    }

    [Fact]
    public async Task UpdateProductAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var updateDto = new UpdateProductDto { Name = "Updated" };

        // Act
        var result = await _service.UpdateProductAsync(999, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProductAsync_WithPartialData_UpdatesOnlySpecifiedFields()
    {
        // Arrange
        var product = new Product
        {
            Name = "Original",
            Description = "Original Description",
            Category = "Test",
            Price = 99.99m
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateProductDto { Name = "Updated" };

        // Act
        var result = await _service.UpdateProductAsync(product.Id, updateDto);

        // Assert
        Assert.Equal("Updated", result!.Name);
        Assert.Equal("Original Description", result.Description);
    }

    [Fact]
    public async Task DeleteProductAsync_WithValidId_DeletesProduct()
    {
        // Arrange
        var product = new Product { Name = "Test", Category = "Test", Price = 99.99m };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteProductAsync(product.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(0, await _context.Products.CountAsync());
    }

    [Fact]
    public async Task DeleteProductAsync_WithInvalidId_ReturnsFalse()
    {
        // Act
        var result = await _service.DeleteProductAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchProductsAsync_ByName_ReturnsMatchingProducts()
    {
        // Arrange
        _context.Products.AddRange(
            new Product { Name = "Laptop Computer", Category = "Electronics", Price = 999m, IsActive = true },
            new Product { Name = "Desktop Computer", Category = "Electronics", Price = 1299m, IsActive = true },
            new Product { Name = "Coffee Mug", Category = "Accessories", Price = 12m, IsActive = true }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchProductsAsync("Computer");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchProductsAsync_ByDescription_ReturnsMatchingProducts()
    {
        // Arrange
        _context.Products.AddRange(
            new Product { Name = "Product 1", Description = "High quality item", Category = "Test", Price = 10m, IsActive = true },
            new Product { Name = "Product 2", Description = "Budget friendly", Category = "Test", Price = 20m, IsActive = true }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchProductsAsync("quality");

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchProductsAsync_ByCategory_ReturnsMatchingProducts()
    {
        // Arrange
        _context.Products.AddRange(
            new Product { Name = "Product 1", Category = "Electronics", Price = 10m, IsActive = true },
            new Product { Name = "Product 2", Category = "Books", Price = 20m, IsActive = true }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchProductsAsync("Electronics");

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchProductsAsync_OnlyReturnsActiveProducts()
    {
        // Arrange
        _context.Products.AddRange(
            new Product { Name = "Active Product", Category = "Test", Price = 10m, IsActive = true },
            new Product { Name = "Inactive Product", Category = "Test", Price = 20m, IsActive = false }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchProductsAsync("Product");

        // Assert
        Assert.Single(result);
        Assert.True(result.First().IsActive);
    }

    [Fact]
    public async Task SearchProductsAsync_CaseInsensitive_ReturnsMatchingProducts()
    {
        // Arrange
        _context.Products.Add(new Product { Name = "Laptop", Category = "Test", Price = 10m, IsActive = true });
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchProductsAsync("LAPTOP");

        // Assert
        Assert.Single(result);
    }
}
