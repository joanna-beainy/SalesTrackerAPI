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

            // CreateSaleItemDto → SaleItem
            CreateMap<CreateSaleItemDto, SaleItem>();

            // Sale → ReadSaleDto
            CreateMap<Sale, ReadSaleDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Sale → ReadSaleV2Dto
            CreateMap<Sale, ReadSaleV2Dto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));

            // SaleItem → ReadSaleItemDto (v1: basic info only)
            CreateMap<SaleItem, ReadSaleItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            // SaleItem → ReadSaleItemV2Dto (v2: includes discount logic)
            CreateMap<SaleItem, ReadSaleItemV2Dto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
                .ForMember(dest => dest.TotalPriceAfterDiscount, opt => opt.MapFrom(src =>
                    Math.Round(src.UnitPrice * src.Quantity * (decimal)(1 - src.DiscountPercentage / 100.0), 2)));
        }
    }
}
