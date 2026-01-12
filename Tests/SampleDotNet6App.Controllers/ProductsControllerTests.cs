using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SampleDotNet6App.Controllers;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleDotNet6App.Controllers.Tests;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly Mock<ILogger<ProductsController>> _loggerMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _loggerMock = new Mock<ILogger<ProductsController>>();
        _controller = new ProductsController(_productServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var controller = new ProductsController(_productServiceMock.Object, _loggerMock.Object);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public async Task GetProducts_ReturnsOkResult()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product 1", Price = 10.99m },
            new ProductDto { Id = 2, Name = "Product 2", Price = 20.99m }
        };
        _productServiceMock.Setup(x => x.GetAllProductsAsync(null, null))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(2, ((List<ProductDto>)returnedProducts).Count);
    }

    [Fact]
    public async Task GetProducts_WithCategoryFilter_ReturnsFilteredProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product 1", Category = "Electronics", Price = 10.99m }
        };
        _productServiceMock.Setup(x => x.GetAllProductsAsync("Electronics", null))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetProducts("Electronics", null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetProducts_WithIsActiveFilter_ReturnsFilteredProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product 1", IsActive = true, Price = 10.99m }
        };
        _productServiceMock.Setup(x => x.GetAllProductsAsync(null, true))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetProducts(null, true);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var product = new ProductDto { Id = 1, Name = "Product 1", Price = 10.99m };
        _productServiceMock.Setup(x => x.GetProductByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal(1, returnedProduct.Id);
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _productServiceMock.Setup(x => x.GetProductByIdAsync(999))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var result = await _controller.GetProduct(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var createDto = new CreateProductDto { Name = "New Product", Price = 15.99m };
        var createdProduct = new ProductDto { Id = 1, Name = "New Product", Price = 15.99m };
        _productServiceMock.Setup(x => x.CreateProductAsync(createDto))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.CreateProduct(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedProduct = Assert.IsType<ProductDto>(createdResult.Value);
        Assert.Equal("New Product", returnedProduct.Name);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateProductDto();
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.CreateProduct(createDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var updateDto = new UpdateProductDto { Name = "Updated Product" };
        var updatedProduct = new ProductDto { Id = 1, Name = "Updated Product", Price = 10.99m };
        _productServiceMock.Setup(x => x.UpdateProductAsync(1, updateDto))
            .ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.UpdateProduct(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal("Updated Product", returnedProduct.Name);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateProductDto { Name = "Updated Product" };
        _productServiceMock.Setup(x => x.UpdateProductAsync(999, updateDto))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var result = await _controller.UpdateProduct(999, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateProductDto();
        _controller.ModelState.AddModelError("Price", "Invalid price");

        // Act
        var result = await _controller.UpdateProduct(1, updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _productServiceMock.Setup(x => x.DeleteProductAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteProduct(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _productServiceMock.Setup(x => x.DeleteProductAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteProduct(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task SearchProducts_WithValidSearchTerm_ReturnsOkResult()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Laptop", Price = 999.99m }
        };
        _productServiceMock.Setup(x => x.SearchProductsAsync("Laptop"))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.SearchProducts("Laptop");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.NotEmpty(returnedProducts);
    }

    [Fact]
    public async Task SearchProducts_WithEmptySearchTerm_ReturnsBadRequest()
    {
        // Arrange & Act
        var result = await _controller.SearchProducts("");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchProducts_WithNullSearchTerm_ReturnsBadRequest()
    {
        // Arrange & Act
        var result = await _controller.SearchProducts(null!);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchProducts_WithWhitespaceSearchTerm_ReturnsBadRequest()
    {
        // Arrange & Act
        var result = await _controller.SearchProducts("   ");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
