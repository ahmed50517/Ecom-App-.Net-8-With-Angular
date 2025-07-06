using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entities.Product;

namespace Ecom.Api.Mapping
{
    public class ProductMapping:Profile
    {
        public ProductMapping()
        {
            CreateMap<Product, ProductDTO>().
                ForMember(x => x.CategoryName,
                options => options.MapFrom(src => src.Name)).ReverseMap();

            CreateMap<Photo, PhotoDTO>().ReverseMap();
            CreateMap<AddProductDTO, Product>()
            .ForMember(m=>m.Photos,op=>op.Ignore()).ReverseMap();

            CreateMap<UpdateCategoryDTO, Product>()
            .ForMember(m => m.Photos, op => op.Ignore()).ReverseMap();
        }
    }
}
