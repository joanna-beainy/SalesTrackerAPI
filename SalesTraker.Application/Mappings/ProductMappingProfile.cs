using AutoMapper;
using SalesTracker.Application.DTOs;
using SalesTracker.InfraStructure.Models.Entities;

namespace SalesTracker.Application.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile(){
            CreateMap<AddProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
            CreateMap<Product, ReadProductDto>();
        }
    }
}
