namespace SampleDotNet6App.DTOs;

/// <summary>
/// Product data transfer object for API responses
/// </summary>
public class ProductDto
{
    /// <summary>
    /// The unique identifier for the product
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the product
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The product description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The price of the product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// The category this product belongs to
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// The current stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Whether the product is active
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Create product request DTO
/// </summary>
public class CreateProductDto
{
    /// <summary>
    /// The name of the product
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The product description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The price of the product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// The category this product belongs to
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// The current stock quantity
    /// </summary>
    public int StockQuantity { get; set; }
}

/// <summary>
/// Update product request DTO
/// </summary>
public class UpdateProductDto
{
    /// <summary>
    /// The name of the product
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The product description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The price of the product
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// The category this product belongs to
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// The current stock quantity
    /// </summary>
    public int? StockQuantity { get; set; }

    /// <summary>
    /// Whether the product is active
    /// </summary>
    public bool? IsActive { get; set; }
}
