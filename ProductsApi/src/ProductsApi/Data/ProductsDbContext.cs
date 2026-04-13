using Microsoft.EntityFrameworkCore;
using ProductsApi.Entities;

namespace ProductsApi.Data;

public class ProductsDbContext : DbContext
{
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
    {
    }

    public DbSet<ProductEntity> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductEntity>().HasData(
            new ProductEntity { Id = 1, Name = "Running Shoes", Description = "Lightweight running shoes", Price = 89.99m, Colour = "Red" },
            new ProductEntity { Id = 2, Name = "Casual T-Shirt", Description = "Cotton crew neck t-shirt", Price = 24.99m, Colour = "Blue" },
            new ProductEntity { Id = 3, Name = "Winter Jacket", Description = "Insulated winter jacket", Price = 149.99m, Colour = "Green" },
            new ProductEntity { Id = 4, Name = "Denim Jeans", Description = "Slim fit denim jeans", Price = 59.99m, Colour = "Blue" },
            new ProductEntity { Id = 5, Name = "Baseball Cap", Description = "Adjustable baseball cap", Price = 19.99m, Colour = "Red" },
            new ProductEntity { Id = 6, Name = "Leather Belt", Description = "Genuine leather belt", Price = 34.99m, Colour = "Black" }
        );
    }
}
