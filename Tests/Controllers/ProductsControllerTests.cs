using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SampleDotNet6App.Controllers;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Services;
using Xunit;

namespace SampleDotNet6App.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<ILogger<ProductsController>> _mockLogger;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _mockLogger = new Mock<ILogger<ProductsController>>();
        _controller = new ProductsController(_mockProductService.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var controller = new ProductsController(_mockProductService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public async Task GetProducts_WithNoFilters_ReturnsOkResult()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product 1" },
            new ProductDto { Id = 2, Name = "Product 2" }
        };
        _mockProductService.Setup(s => s.GetAllProductsAsync(null, null))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetProducts_WithCategoryFilter_ReturnsFilteredProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product 1", Category = "Electronics" }
        };
        _mockProductService.Setup(s => s.GetAllProductsAsync("Electronics", null))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetProducts("Electronics");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task GetProducts_WithIsActiveFilter_ReturnsFilteredProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product 1", IsActive = true }
        };
        _mockProductService.Setup(s => s.GetAllProductsAsync(null, true))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetProducts(null, true);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsProduct()
    {
        // Arrange
        var product = new ProductDto { Id = 1, Name = "Product 1" };
        _mockProductService.Setup(s => s.GetProductByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetProductByIdAsync(999))
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
        var createDto = new CreateProductDto { Name = "New Product", Price = 99.99m };
        var createdProduct = new ProductDto { Id = 1, Name = "New Product", Price = 99.99m };
        _mockProductService.Setup(s => s.CreateProductAsync(createDto))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.CreateProduct(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<ProductDto>(createdResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateProductDto();
        _controller.ModelState.AddModelError("Name", "Required");

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
        var updatedProduct = new ProductDto { Id = 1, Name = "Updated Product" };
        _mockProductService.Setup(s => s.UpdateProductAsync(1, updateDto))
            .ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.UpdateProduct(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal("Updated Product", returnValue.Name);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateProductDto { Name = "Updated Product" };
        _mockProductService.Setup(s => s.UpdateProductAsync(999, updateDto))
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
        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.UpdateProduct(1, updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockProductService.Setup(s => s.DeleteProductAsync(1))
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
        _mockProductService.Setup(s => s.DeleteProductAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteProduct(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task SearchProducts_WithValidQuery_ReturnsProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Laptop" }
        };
        _mockProductService.Setup(s => s.SearchProductsAsync("Laptop"))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.SearchProducts("Laptop");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task SearchProducts_WithEmptyQuery_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.SearchProducts("");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchProducts_WithWhitespaceQuery_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.SearchProducts("   ");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
