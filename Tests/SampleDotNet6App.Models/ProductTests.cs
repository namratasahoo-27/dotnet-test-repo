using Xunit;
using SampleDotNet6App.Models;
using System;

namespace SampleDotNet6App.Models.Tests;

public class ProductTests
{
    [Fact]
    public void Product_CanBeInstantiated()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        Assert.NotNull(product);
    }

    [Fact]
    public void Product_PropertiesCanBeSet()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Category = "Test Category",
            StockQuantity = 50,
            CreatedAt = now,
            UpdatedAt = now,
            IsActive = true
        };

        // Assert
        Assert.Equal(1, product.Id);
        Assert.Equal("Test Product", product.Name);
        Assert.Equal("Test Description", product.Description);
        Assert.Equal(99.99m, product.Price);
        Assert.Equal("Test Category", product.Category);
        Assert.Equal(50, product.StockQuantity);
        Assert.Equal(now, product.CreatedAt);
        Assert.Equal(now, product.UpdatedAt);
        Assert.True(product.IsActive);
    }

    [Fact]
    public void Product_DefaultValues()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        Assert.Equal(0, product.Id);
        Assert.Equal(string.Empty, product.Name);
        Assert.Equal(string.Empty, product.Description);
        Assert.Equal(0m, product.Price);
        Assert.Equal(string.Empty, product.Category);
        Assert.Equal(0, product.StockQuantity);
        Assert.Equal(default(DateTime), product.CreatedAt);
        Assert.Equal(default(DateTime), product.UpdatedAt);
        Assert.True(product.IsActive);
    }

    [Fact]
    public void Product_IsActiveDefaultsToTrue()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        Assert.True(product.IsActive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999999)]
    public void Product_IdProperty_AcceptsVariousValues(int id)
    {
        // Arrange
        var product = new Product { Id = id };

        // Assert
        Assert.Equal(id, product.Id);
    }

    [Theory]
    [InlineData("Product A")]
    [InlineData("Product B")]
    [InlineData("")]
    public void Product_NameProperty_AcceptsVariousValues(string name)
    {
        // Arrange
        var product = new Product { Name = name };

        // Assert
        Assert.Equal(name, product.Name);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(10.99)]
    [InlineData(999.99)]
    [InlineData(9999.99)]
    public void Product_PriceProperty_AcceptsPositiveValues(decimal price)
    {
        // Arrange
        var product = new Product { Price = price };

        // Assert
        Assert.Equal(price, product.Price);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Product_StockQuantityProperty_AcceptsVariousValues(int stock)
    {
        // Arrange
        var product = new Product { StockQuantity = stock };

        // Assert
        Assert.Equal(stock, product.StockQuantity);
    }

    [Theory]
    [InlineData("Electronics")]
    [InlineData("Furniture")]
    [InlineData("Accessories")]
    public void Product_CategoryProperty_AcceptsVariousCategories(string category)
    {
        // Arrange
        var product = new Product { Category = category };

        // Assert
        Assert.Equal(category, product.Category);
    }

    [Fact]
    public void Product_CreatedAtAndUpdatedAt_CanBeDifferent()
    {
        // Arrange
        var createdAt = DateTime.UtcNow.AddDays(-7);
        var updatedAt = DateTime.UtcNow;
        var product = new Product
        {
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        // Assert
        Assert.Equal(createdAt, product.CreatedAt);
        Assert.Equal(updatedAt, product.UpdatedAt);
        Assert.NotEqual(product.CreatedAt, product.UpdatedAt);
    }

    [Fact]
    public void Product_IsActiveProperty_CanBeToggled()
    {
        // Arrange
        var product = new Product { IsActive = true };

        // Act
        product.IsActive = false;

        // Assert
        Assert.False(product.IsActive);
    }

    [Fact]
    public void Product_DescriptionProperty_CanBeEmpty()
    {
        // Arrange
        var product = new Product { Description = "" };

        // Assert
        Assert.Equal(string.Empty, product.Description);
    }

    [Fact]
    public void Product_AllPropertiesCanBeModified()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Original",
            Description = "Original Description",
            Price = 10m,
            Category = "Original Category",
            StockQuantity = 5,
            IsActive = true
        };

        // Act
        product.Id = 2;
        product.Name = "Modified";
        product.Description = "Modified Description";
        product.Price = 20m;
        product.Category = "Modified Category";
        product.StockQuantity = 10;
        product.IsActive = false;

        // Assert
        Assert.Equal(2, product.Id);
        Assert.Equal("Modified", product.Name);
        Assert.Equal("Modified Description", product.Description);
        Assert.Equal(20m, product.Price);
        Assert.Equal("Modified Category", product.Category);
        Assert.Equal(10, product.StockQuantity);
        Assert.False(product.IsActive);
    }
}
