using Xunit;
using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.DTOs.Tests;

public class ProductDtoTests
{
    [Fact]
    public void ProductDto_CanBeInstantiated()
    {
        // Arrange & Act
        var dto = new ProductDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void ProductDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new ProductDto
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Category = "Test Category",
            StockQuantity = 10,
            IsActive = true
        };

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("Test Product", dto.Name);
        Assert.Equal("Test Description", dto.Description);
        Assert.Equal(99.99m, dto.Price);
        Assert.Equal("Test Category", dto.Category);
        Assert.Equal(10, dto.StockQuantity);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void ProductDto_DefaultValues()
    {
        // Arrange & Act
        var dto = new ProductDto();

        // Assert
        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Description);
        Assert.Equal(0m, dto.Price);
        Assert.Equal(string.Empty, dto.Category);
        Assert.Equal(0, dto.StockQuantity);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void CreateProductDto_CanBeInstantiated()
    {
        // Arrange & Act
        var dto = new CreateProductDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void CreateProductDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "New Product",
            Description = "New Description",
            Price = 49.99m,
            Category = "New Category",
            StockQuantity = 5
        };

        // Assert
        Assert.Equal("New Product", dto.Name);
        Assert.Equal("New Description", dto.Description);
        Assert.Equal(49.99m, dto.Price);
        Assert.Equal("New Category", dto.Category);
        Assert.Equal(5, dto.StockQuantity);
    }

    [Fact]
    public void CreateProductDto_DefaultValues()
    {
        // Arrange & Act
        var dto = new CreateProductDto();

        // Assert
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Description);
        Assert.Equal(0m, dto.Price);
        Assert.Equal(string.Empty, dto.Category);
        Assert.Equal(0, dto.StockQuantity);
    }

    [Fact]
    public void UpdateProductDto_CanBeInstantiated()
    {
        // Arrange & Act
        var dto = new UpdateProductDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void UpdateProductDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new UpdateProductDto
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 79.99m,
            Category = "Updated Category",
            StockQuantity = 15,
            IsActive = false
        };

        // Assert
        Assert.Equal("Updated Product", dto.Name);
        Assert.Equal("Updated Description", dto.Description);
        Assert.Equal(79.99m, dto.Price);
        Assert.Equal("Updated Category", dto.Category);
        Assert.Equal(15, dto.StockQuantity);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void UpdateProductDto_DefaultValues()
    {
        // Arrange & Act
        var dto = new UpdateProductDto();

        // Assert
        Assert.Null(dto.Name);
        Assert.Null(dto.Description);
        Assert.Null(dto.Price);
        Assert.Null(dto.Category);
        Assert.Null(dto.StockQuantity);
        Assert.Null(dto.IsActive);
    }

    [Fact]
    public void UpdateProductDto_NullablePropertiesCanBeNull()
    {
        // Arrange
        var dto = new UpdateProductDto
        {
            Name = null,
            Description = null,
            Price = null,
            Category = null,
            StockQuantity = null,
            IsActive = null
        };

        // Assert
        Assert.Null(dto.Name);
        Assert.Null(dto.Description);
        Assert.Null(dto.Price);
        Assert.Null(dto.Category);
        Assert.Null(dto.StockQuantity);
        Assert.Null(dto.IsActive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void ProductDto_IdProperty_AcceptsVariousValues(int id)
    {
        // Arrange
        var dto = new ProductDto { Id = id };

        // Assert
        Assert.Equal(id, dto.Id);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(10.99)]
    [InlineData(999.99)]
    [InlineData(9999.99)]
    public void ProductDto_PriceProperty_AcceptsVariousValues(decimal price)
    {
        // Arrange
        var dto = new ProductDto { Price = price };

        // Assert
        Assert.Equal(price, dto.Price);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    public void ProductDto_StockQuantityProperty_AcceptsVariousValues(int stock)
    {
        // Arrange
        var dto = new ProductDto { StockQuantity = stock };

        // Assert
        Assert.Equal(stock, dto.StockQuantity);
    }
}
