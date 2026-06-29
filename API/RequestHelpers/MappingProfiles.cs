
using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.RequestHelpers;
public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
       
        CreateMap<Product, ProductDto>();
        CreateMap<BasketItem, BasketItemDto>();
        CreateMap<Basket, BasketDto>();

        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
        
    }
}