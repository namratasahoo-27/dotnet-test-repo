namespace SampleDotNet6App.Models;

/// <summary>
/// Represents a product in the inventory
/// </summary>
public class Product
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
    /// When the product was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the product was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Whether the product is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
