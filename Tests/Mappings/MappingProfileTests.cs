using AutoMapper;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;
using Xunit;

namespace SampleDotNet6App.Tests.Mappings;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void MappingProfile_Configuration_IsValid()
    {
        // Arrange & Act & Assert
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_ProductToProductDto_MapsCorrectly()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Category = "Test",
            StockQuantity = 10,
            IsActive = true
        };

        // Act
        var dto = _mapper.Map<ProductDto>(product);

        // Assert
        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(product.Name, dto.Name);
        Assert.Equal(product.Description, dto.Description);
        Assert.Equal(product.Price, dto.Price);
        Assert.Equal(product.Category, dto.Category);
        Assert.Equal(product.StockQuantity, dto.StockQuantity);
        Assert.Equal(product.IsActive, dto.IsActive);
    }

    [Fact]
    public void Map_CreateProductDtoToProduct_MapsCorrectly()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "New Product",
            Description = "New Description",
            Price = 49.99m,
            Category = "New",
            StockQuantity = 5
        };

        // Act
        var product = _mapper.Map<Product>(createDto);

        // Assert
        Assert.Equal(createDto.Name, product.Name);
        Assert.Equal(createDto.Description, product.Description);
        Assert.Equal(createDto.Price, product.Price);
        Assert.Equal(createDto.Category, product.Category);
        Assert.Equal(createDto.StockQuantity, product.StockQuantity);
        Assert.True(product.IsActive);
    }

    [Fact]
    public void Map_CreateProductDtoToProduct_IgnoresId()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "New Product",
            Price = 49.99m,
            Category = "New"
        };

        // Act
        var product = _mapper.Map<Product>(createDto);

        // Assert
        Assert.Equal(0, product.Id);
    }

    [Fact]
    public void Map_CreateProductDtoToProduct_SetsTimestamps()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "New Product",
            Price = 49.99m,
            Category = "New"
        };
        var beforeMapping = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var product = _mapper.Map<Product>(createDto);
        var afterMapping = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(product.CreatedAt >= beforeMapping && product.CreatedAt <= afterMapping);
        Assert.True(product.UpdatedAt >= beforeMapping && product.UpdatedAt <= afterMapping);
    }

    [Fact]
    public void Map_UpdateProductDtoToProduct_MapsNonNullProperties()
    {
        // Arrange
        var existingProduct = new Product
        {
            Id = 1,
            Name = "Original",
            Description = "Original Description",
            Price = 99.99m,
            Category = "Original",
            StockQuantity = 10
        };
        var updateDto = new UpdateProductDto
        {
            Name = "Updated Name",
            Price = 79.99m
        };

        // Act
        _mapper.Map(updateDto, existingProduct);

        // Assert
        Assert.Equal("Updated Name", existingProduct.Name);
        Assert.Equal(79.99m, existingProduct.Price);
        Assert.Equal("Original Description", existingProduct.Description);
    }

    [Fact]
    public void Map_UserToUserDto_MapsCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Role = UserRole.User,
            IsActive = true
        };

        // Act
        var dto = _mapper.Map<UserDto>(user);

        // Assert
        Assert.Equal(user.Id, dto.Id);
        Assert.Equal(user.Username, dto.Username);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.FirstName, dto.FirstName);
        Assert.Equal(user.LastName, dto.LastName);
        Assert.Equal(user.Role, dto.Role);
        Assert.Equal(user.IsActive, dto.IsActive);
    }

    [Fact]
    public void Map_RegisterDtoToUser_MapsCorrectly()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@test.com",
            FirstName = "New",
            LastName = "User",
            Password = "password"
        };

        // Act
        var user = _mapper.Map<User>(registerDto);

        // Assert
        Assert.Equal(registerDto.Username, user.Username);
        Assert.Equal(registerDto.Email, user.Email);
        Assert.Equal(registerDto.FirstName, user.FirstName);
        Assert.Equal(registerDto.LastName, user.LastName);
        Assert.Equal(UserRole.User, user.Role);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Map_RegisterDtoToUser_IgnoresId()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@test.com",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        var user = _mapper.Map<User>(registerDto);

        // Assert
        Assert.Equal(0, user.Id);
    }

    [Fact]
    public void Map_RegisterDtoToUser_IgnoresPasswordHash()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@test.com",
            Password = "password"
        };

        // Act
        var user = _mapper.Map<User>(registerDto);

        // Assert
        Assert.Equal(string.Empty, user.PasswordHash);
    }

    [Fact]
    public void Map_MultipleProducts_MapsCorrectly()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 10m, Category = "Test" },
            new Product { Id = 2, Name = "Product 2", Price = 20m, Category = "Test" }
        };

        // Act
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);

        // Assert
        Assert.Equal(2, dtos.Count());
        Assert.Equal("Product 1", dtos.First().Name);
        Assert.Equal("Product 2", dtos.Last().Name);
    }

    [Fact]
    public void Map_MultipleUsers_MapsCorrectly()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Username = "user1", Email = "user1@test.com" },
            new User { Id = 2, Username = "user2", Email = "user2@test.com" }
        };

        // Act
        var dtos = _mapper.Map<IEnumerable<UserDto>>(users);

        // Assert
        Assert.Equal(2, dtos.Count());
        Assert.Equal("user1", dtos.First().Username);
        Assert.Equal("user2", dtos.Last().Username);
    }
}
