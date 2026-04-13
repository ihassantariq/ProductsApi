using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.IRepository;
using ProductsApi.Models;

namespace ProductsApi.Controllers;

[ApiController]
[Route("api/Products")]
[Authorize]
public class ProductsController : Controller
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet("GetAllProducts", Name = "Products-GetAllProducts")]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<ProductResponse>> GetAllProducts([FromQuery] string? colour)
    {
        return await _productRepository.GetAllProductsAsync(colour);
    }

    [HttpPost("CreateProduct", Name = "Products-CreateProduct")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var product = await _productRepository.CreateProductAsync(request);
        return CreatedAtRoute("Products-GetAllProducts", null, product);
    }
}
