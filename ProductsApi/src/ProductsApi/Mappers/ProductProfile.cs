using AutoMapper;
using ProductsApi.Entities;
using ProductsApi.Models;

namespace ProductsApi.Mappers;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductEntity, ProductResponse>();
        CreateMap<CreateProductRequest, ProductEntity>();
    }
}
