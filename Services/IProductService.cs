using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.Services;

/// <summary>
/// Product service interface
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Gets all products
    /// </summary>
    /// <param name="category">Optional category filter</param>
    /// <param name="isActive">Optional active status filter</param>
    /// <returns>List of products</returns>
    Task<IEnumerable<ProductDto>> GetAllProductsAsync(string? category = null, bool? isActive = null);

    /// <summary>
    /// Gets a product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product if found, null otherwise</returns>
    Task<ProductDto?> GetProductByIdAsync(int id);

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="createProductDto">Product creation data</param>
    /// <returns>Created product</returns>
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="updateProductDto">Product update data</param>
    /// <returns>Updated product if found, null otherwise</returns>
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteProductAsync(int id);

    /// <summary>
    /// Searches products by name
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>Matching products</returns>
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
}
