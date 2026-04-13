using ProductsApi.Models;

namespace ProductsApi.IRepository;

public interface IProductRepository
{
    /// <summary>
    /// Get all products, optionally filtered by colour
    /// </summary>
    Task<List<ProductResponse>> GetAllProductsAsync(string? colour);

    /// <summary>
    /// Create a new product
    /// </summary>
    Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
}
