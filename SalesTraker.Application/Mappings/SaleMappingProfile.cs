using AutoMapper;
using SalesTracker.Application.DTOs;
using SalesTracker.InfraStructure.Models.Entities;

namespace SalesTracker.Application.Mappings
{
    public class SaleMappingProfile : Profile
    {
        public SaleMappingProfile()
        {
            // CreateSaleDto → Sale (ignore SaleItems, enrich manually)
            CreateMap<CreateSaleDto, Sale>()
                .ForMember(dest => dest.SaleItems, opt => opt.Ignore());

            // CreateSaleItemDto → SaleItem (basic mapping, enrich UnitPrice manually)
            CreateMap<CreateSaleItemDto, SaleItem>();

            // Sale → ReadSaleDto
            CreateMap<Sale, ReadSaleDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));

            // SaleItem → ReadSaleItemDto
            CreateMap<SaleItem, ReadSaleItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<Sale, ReadSaleDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        }
    }
}
