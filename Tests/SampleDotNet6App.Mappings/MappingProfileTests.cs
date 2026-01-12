using Xunit;
using AutoMapper;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;
using SampleDotNet6App.DTOs;
using System;

namespace SampleDotNet6App.Mappings.Tests;

public class MappingProfileTests
{
    private readonly IMapper _mapper;
    private readonly MapperConfiguration _configuration;

    public MappingProfileTests()
    {
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = _configuration.CreateMapper();
    }

    [Fact]
    public void MappingProfile_Configuration_IsValid()
    {
        // Arrange & Act & Assert
        _configuration.AssertConfigurationIsValid();
    }

    [Fact]
    public void ProductToProductDto_MapsCorrectly()
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
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.Map<ProductDto>(product);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(product.Name, dto.Name);
        Assert.Equal(product.Description, dto.Description);
        Assert.Equal(product.Price, dto.Price);
        Assert.Equal(product.Category, dto.Category);
        Assert.Equal(product.StockQuantity, dto.StockQuantity);
        Assert.Equal(product.IsActive, dto.IsActive);
    }

    [Fact]
    public void CreateProductDtoToProduct_MapsCorrectly()
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
        Assert.NotNull(product);
        Assert.Equal(0, product.Id);
        Assert.Equal(createDto.Name, product.Name);
        Assert.Equal(createDto.Description, product.Description);
        Assert.Equal(createDto.Price, product.Price);
        Assert.Equal(createDto.Category, product.Category);
        Assert.Equal(createDto.StockQuantity, product.StockQuantity);
        Assert.True(product.IsActive);
        Assert.True(product.CreatedAt > DateTime.MinValue);
        Assert.True(product.UpdatedAt > DateTime.MinValue);
    }

    [Fact]
    public void UpdateProductDtoToProduct_MapsOnlyNonNullProperties()
    {
        // Arrange
        var existingProduct = new Product
        {
            Id = 1,
            Name = "Old Name",
            Description = "Old Description",
            Price = 50m,
            Category = "Old",
            StockQuantity = 10,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updateDto = new UpdateProductDto
        {
            Name = "New Name",
            Price = 75m
        };

        // Act
        _mapper.Map(updateDto, existingProduct);

        // Assert
        Assert.Equal("New Name", existingProduct.Name);
        Assert.Equal(75m, existingProduct.Price);
        Assert.Equal("Old Description", existingProduct.Description);
        Assert.Equal("Old", existingProduct.Category);
        Assert.Equal(0, existingProduct.StockQuantity);
    }

    [Fact]
    public void UpdateProductDtoToProduct_DoesNotMapIdOrCreatedAt()
    {
        // Arrange
        var originalCreatedAt = DateTime.UtcNow.AddDays(-10);
        var existingProduct = new Product
        {
            Id = 1,
            Name = "Test",
            CreatedAt = originalCreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var updateDto = new UpdateProductDto { Name = "Updated" };

        // Act
        _mapper.Map(updateDto, existingProduct);

        // Assert
        Assert.Equal(1, existingProduct.Id);
        Assert.Equal(originalCreatedAt, existingProduct.CreatedAt);
    }

    [Fact]
    public void UserToUserDto_MapsCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash123",
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.Map<UserDto>(user);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(user.Id, dto.Id);
        Assert.Equal(user.Username, dto.Username);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.FirstName, dto.FirstName);
        Assert.Equal(user.LastName, dto.LastName);
        Assert.Equal(user.Role, dto.Role);
        Assert.Equal(user.IsActive, dto.IsActive);
    }

    [Fact]
    public void RegisterDtoToUser_MapsCorrectly()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@example.com",
            FirstName = "New",
            LastName = "User",
            Password = "password123"
        };

        // Act
        var user = _mapper.Map<User>(registerDto);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(0, user.Id);
        Assert.Equal(registerDto.Username, user.Username);
        Assert.Equal(registerDto.Email, user.Email);
        Assert.Equal(registerDto.FirstName, user.FirstName);
        Assert.Equal(registerDto.LastName, user.LastName);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(UserRole.User, user.Role);
        Assert.True(user.IsActive);
        Assert.True(user.CreatedAt > DateTime.MinValue);
    }

    [Fact]
    public void RegisterDtoToUser_DoesNotMapPassword()
    {
        // Arrange
        var registerDto = new RegisterDto { Password = "password123" };

        // Act
        var user = _mapper.Map<User>(registerDto);

        // Assert
        Assert.NotEqual("password123", user.PasswordHash);
        Assert.Equal(string.Empty, user.PasswordHash);
    }

    [Fact]
    public void RegisterDtoToUser_SetsDefaultRole()
    {
        // Arrange
        var registerDto = new RegisterDto { Username = "test" };

        // Act
        var user = _mapper.Map<User>(registerDto);

        // Assert
        Assert.Equal(UserRole.User, user.Role);
    }

    [Fact]
    public void RegisterDtoToUser_SetsIsActiveTrue()
    {
        // Arrange
        var registerDto = new RegisterDto { Username = "test" };

        // Act
        var user = _mapper.Map<User>(registerDto);

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void CreateProductDtoToProduct_SetsIsActiveTrue()
    {
        // Arrange
        var createDto = new CreateProductDto { Name = "Test" };

        // Act
        var product = _mapper.Map<Product>(createDto);

        // Assert
        Assert.True(product.IsActive);
    }
}
