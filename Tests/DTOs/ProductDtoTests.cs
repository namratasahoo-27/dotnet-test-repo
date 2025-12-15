using SampleDotNet6App.DTOs;
using Xunit;

namespace SampleDotNet6App.Tests.DTOs;

public class ProductDtoTests
{
    [Fact]
    public void ProductDto_DefaultConstructor_CreatesInstance()
    {
        // Act
        var dto = new ProductDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Description);
        Assert.Equal(0m, dto.Price);
        Assert.Equal(string.Empty, dto.Category);
        Assert.Equal(0, dto.StockQuantity);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void ProductDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange
        var dto = new ProductDto();

        // Act
        dto.Id = 1;
        dto.Name = "Test Product";
        dto.Description = "Test Description";
        dto.Price = 99.99m;
        dto.Category = "Electronics";
        dto.StockQuantity = 10;
        dto.IsActive = true;

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("Test Product", dto.Name);
        Assert.Equal("Test Description", dto.Description);
        Assert.Equal(99.99m, dto.Price);
        Assert.Equal("Electronics", dto.Category);
        Assert.Equal(10, dto.StockQuantity);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void ProductDto_WithNegativePrice_SetsPrice()
    {
        // Arrange
        var dto = new ProductDto();

        // Act
        dto.Price = -10.50m;

        // Assert
        Assert.Equal(-10.50m, dto.Price);
    }

    [Fact]
    public void ProductDto_WithLargePrice_SetsPrice()
    {
        // Arrange
        var dto = new ProductDto();

        // Act
        dto.Price = 999999.99m;

        // Assert
        Assert.Equal(999999.99m, dto.Price);
    }

    [Fact]
    public void CreateProductDto_DefaultConstructor_CreatesInstance()
    {
        // Act
        var dto = new CreateProductDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Description);
        Assert.Equal(0m, dto.Price);
        Assert.Equal(string.Empty, dto.Category);
        Assert.Equal(0, dto.StockQuantity);
    }

    [Fact]
    public void CreateProductDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange
        var dto = new CreateProductDto();

        // Act
        dto.Name = "New Product";
        dto.Description = "New Description";
        dto.Price = 49.99m;
        dto.Category = "Books";
        dto.StockQuantity = 5;

        // Assert
        Assert.Equal("New Product", dto.Name);
        Assert.Equal("New Description", dto.Description);
        Assert.Equal(49.99m, dto.Price);
        Assert.Equal("Books", dto.Category);
        Assert.Equal(5, dto.StockQuantity);
    }

    [Fact]
    public void UpdateProductDto_DefaultConstructor_CreatesInstance()
    {
        // Act
        var dto = new UpdateProductDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Null(dto.Name);
        Assert.Null(dto.Description);
        Assert.Null(dto.Price);
        Assert.Null(dto.Category);
        Assert.Null(dto.StockQuantity);
        Assert.Null(dto.IsActive);
    }

    [Fact]
    public void UpdateProductDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange
        var dto = new UpdateProductDto();

        // Act
        dto.Name = "Updated Product";
        dto.Description = "Updated Description";
        dto.Price = 79.99m;
        dto.Category = "Furniture";
        dto.StockQuantity = 15;
        dto.IsActive = false;

        // Assert
        Assert.Equal("Updated Product", dto.Name);
        Assert.Equal("Updated Description", dto.Description);
        Assert.Equal(79.99m, dto.Price);
        Assert.Equal("Furniture", dto.Category);
        Assert.Equal(15, dto.StockQuantity);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void UpdateProductDto_PartialUpdate_OnlySetsSpecifiedProperties()
    {
        // Arrange
        var dto = new UpdateProductDto();

        // Act
        dto.Name = "Updated Name";
        dto.Price = 59.99m;

        // Assert
        Assert.Equal("Updated Name", dto.Name);
        Assert.Equal(59.99m, dto.Price);
        Assert.Null(dto.Description);
        Assert.Null(dto.Category);
        Assert.Null(dto.StockQuantity);
        Assert.Null(dto.IsActive);
    }
}
