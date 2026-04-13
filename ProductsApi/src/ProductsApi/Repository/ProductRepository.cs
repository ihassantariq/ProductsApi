using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Entities;
using ProductsApi.IRepository;
using ProductsApi.Models;

namespace ProductsApi.Repository;

public class ProductRepository : IProductRepository
{
    private readonly ProductsDbContext _dbContext;
    private readonly IMapper _mapper;

    public ProductRepository(ProductsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all products, optionally filtered by colour
    /// </summary>
    public async Task<List<ProductResponse>> GetAllProductsAsync(string? colour)
    {
        var query = _dbContext.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(colour))
        {
            query = query.Where(p => p.Colour.ToLower() == colour.ToLower());
        }

        var products = await query.ToListAsync();

        return _mapper.Map<List<ProductResponse>>(products);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
    {
        var entity = _mapper.Map<ProductEntity>(request);

        await _dbContext.Products.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<ProductResponse>(entity);
    }
}
