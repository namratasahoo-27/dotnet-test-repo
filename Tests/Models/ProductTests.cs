using SampleDotNet6App.Models;
using Xunit;

namespace SampleDotNet6App.Tests.Models;

public class ProductTests
{
    [Fact]
    public void Product_DefaultConstructor_CreatesInstance()
    {
        // Act
        var product = new Product();

        // Assert
        Assert.NotNull(product);
        Assert.Equal(0, product.Id);
        Assert.Equal(string.Empty, product.Name);
        Assert.Equal(string.Empty, product.Description);
        Assert.Equal(0m, product.Price);
        Assert.Equal(string.Empty, product.Category);
        Assert.Equal(0, product.StockQuantity);
        Assert.True(product.IsActive);
    }

    [Fact]
    public void Product_SetProperties_ReturnsCorrectValues()
    {
        // Arrange
        var product = new Product();
        var now = DateTime.UtcNow;

        // Act
        product.Id = 1;
        product.Name = "Test Product";
        product.Description = "Test Description";
        product.Price = 99.99m;
        product.Category = "Electronics";
        product.StockQuantity = 10;
        product.CreatedAt = now;
        product.UpdatedAt = now;
        product.IsActive = false;

        // Assert
        Assert.Equal(1, product.Id);
        Assert.Equal("Test Product", product.Name);
        Assert.Equal("Test Description", product.Description);
        Assert.Equal(99.99m, product.Price);
        Assert.Equal("Electronics", product.Category);
        Assert.Equal(10, product.StockQuantity);
        Assert.Equal(now, product.CreatedAt);
        Assert.Equal(now, product.UpdatedAt);
        Assert.False(product.IsActive);
    }

    [Fact]
    public void Product_WithZeroPrice_SetsPrice()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Price = 0m;

        // Assert
        Assert.Equal(0m, product.Price);
    }

    [Fact]
    public void Product_WithNegativePrice_SetsPrice()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Price = -10m;

        // Assert
        Assert.Equal(-10m, product.Price);
    }

    [Fact]
    public void Product_WithLargePrice_SetsPrice()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Price = 999999999.99m;

        // Assert
        Assert.Equal(999999999.99m, product.Price);
    }

    [Fact]
    public void Product_WithNegativeStockQuantity_SetsQuantity()
    {
        // Arrange
        var product = new Product();

        // Act
        product.StockQuantity = -5;

        // Assert
        Assert.Equal(-5, product.StockQuantity);
    }

    [Fact]
    public void Product_WithZeroStockQuantity_SetsQuantity()
    {
        // Arrange
        var product = new Product();

        // Act
        product.StockQuantity = 0;

        // Assert
        Assert.Equal(0, product.StockQuantity);
    }

    [Fact]
    public void Product_WithLargeStockQuantity_SetsQuantity()
    {
        // Arrange
        var product = new Product();

        // Act
        product.StockQuantity = 1000000;

        // Assert
        Assert.Equal(1000000, product.StockQuantity);
    }

    [Fact]
    public void Product_DefaultIsActive_IsTrue()
    {
        // Act
        var product = new Product();

        // Assert
        Assert.True(product.IsActive);
    }

    [Fact]
    public void Product_SetIsActiveToFalse_UpdatesProperty()
    {
        // Arrange
        var product = new Product { IsActive = true };

        // Act
        product.IsActive = false;

        // Assert
        Assert.False(product.IsActive);
    }

    [Fact]
    public void Product_WithEmptyName_SetsName()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Name = "";

        // Assert
        Assert.Equal("", product.Name);
    }

    [Fact]
    public void Product_WithLongDescription_SetsDescription()
    {
        // Arrange
        var product = new Product();
        var longDescription = new string('a', 1000);

        // Act
        product.Description = longDescription;

        // Assert
        Assert.Equal(longDescription, product.Description);
    }

    [Fact]
    public void Product_UpdatedAtAfterCreatedAt_IsValid()
    {
        // Arrange
        var product = new Product();
        var createdAt = DateTime.UtcNow;
        var updatedAt = createdAt.AddHours(1);

        // Act
        product.CreatedAt = createdAt;
        product.UpdatedAt = updatedAt;

        // Assert
        Assert.True(product.UpdatedAt > product.CreatedAt);
    }

    [Fact]
    public void Product_WithDifferentCategories_SetsCategory()
    {
        // Arrange
        var product = new Product();

        // Act & Assert
        product.Category = "Electronics";
        Assert.Equal("Electronics", product.Category);

        product.Category = "Books";
        Assert.Equal("Books", product.Category);

        product.Category = "Furniture";
        Assert.Equal("Furniture", product.Category);
    }
}
