using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Entities;
using ProductsApi.Mappers;
using ProductsApi.Models;
using ProductsApi.Repository;

namespace ProductsApi.Tests.Unit;

public class ProductRepositoryTests
{
    private readonly IMapper _mapper;

    public ProductRepositoryTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductProfile>();
        });
        _mapper = config.CreateMapper();
    }

    private static ProductsDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ProductsDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new ProductsDbContext(options);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        // Arrange
        var context = CreateDbContext("GetAllProducts");
        context.Products.AddRange(
            new ProductEntity { Id = 1, Name = "Product 1", Description = "Desc 1", Price = 10.00m, Colour = "Red" },
            new ProductEntity { Id = 2, Name = "Product 2", Description = "Desc 2", Price = 20.00m, Colour = "Blue" },
            new ProductEntity { Id = 3, Name = "Product 3", Description = "Desc 3", Price = 30.00m, Colour = "Red" }
        );
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context, _mapper);

        // Act
        var result = await repository.GetAllProductsAsync(null);

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetAllProductsAsync_FiltersByColour()
    {
        // Arrange
        var context = CreateDbContext("FilterByColour");
        context.Products.AddRange(
            new ProductEntity { Id = 1, Name = "Red Shoe", Description = "Desc", Price = 10.00m, Colour = "Red" },
            new ProductEntity { Id = 2, Name = "Blue Shirt", Description = "Desc", Price = 20.00m, Colour = "Blue" },
            new ProductEntity { Id = 3, Name = "Red Hat", Description = "Desc", Price = 15.00m, Colour = "Red" }
        );
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context, _mapper);

        // Act
        var result = await repository.GetAllProductsAsync("Red");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.Equal("Red", p.Colour));
    }

    [Fact]
    public async Task GetAllProductsAsync_FiltersByColour_CaseInsensitive()
    {
        // Arrange
        var context = CreateDbContext("FilterCaseInsensitive");
        context.Products.AddRange(
            new ProductEntity { Id = 1, Name = "Red Shoe", Description = "Desc", Price = 10.00m, Colour = "Red" },
            new ProductEntity { Id = 2, Name = "Blue Shirt", Description = "Desc", Price = 20.00m, Colour = "Blue" }
        );
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context, _mapper);

        // Act
        var result = await repository.GetAllProductsAsync("red");

        // Assert
        Assert.Single(result);
        Assert.Equal("Red", result.First().Colour);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsEmpty_WhenNoMatchingColour()
    {
        // Arrange
        var context = CreateDbContext("NoMatchingColour");
        context.Products.Add(
            new ProductEntity { Id = 1, Name = "Red Shoe", Description = "Desc", Price = 10.00m, Colour = "Red" }
        );
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context, _mapper);

        // Act
        var result = await repository.GetAllProductsAsync("Yellow");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateProductAsync_CreatesAndReturnsProduct()
    {
        // Arrange
        var context = CreateDbContext("CreateProduct");
        var repository = new ProductRepository(context, _mapper);

        var request = new CreateProductRequest
        {
            Name = "New Product",
            Description = "A new product",
            Price = 49.99m,
            Colour = "Green"
        };

        // Act
        var result = await repository.CreateProductAsync(request);

        // Assert
        Assert.Equal("New Product", result.Name);
        Assert.Equal("A new product", result.Description);
        Assert.Equal(49.99m, result.Price);
        Assert.Equal("Green", result.Colour);
        Assert.True(result.Id > 0);

        // Verify persisted
        var entity = await context.Products.FirstOrDefaultAsync(p => p.Id == result.Id);
        Assert.NotNull(entity);
        Assert.Equal("New Product", entity.Name);
    }
}
