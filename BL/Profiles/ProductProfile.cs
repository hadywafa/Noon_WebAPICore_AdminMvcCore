using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BL.Helpers;
using BL.ViewModels.RequestVModels;
using EFModel.Models.EFModels;

namespace BL.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // target object  => source object
            CreateMap<Product, VmProduct>()
                .ForMember(dst => dst.Id, src => src.MapFrom(p => p.Id))
                .ForMember(dst => dst.ModelNumber, src => src.MapFrom(p => p.ModelNumber))
                .ForMember(dst => dst.Name, src => src.MapFrom(p => p.Name))
                .ForMember(dst => dst.NameArabic, src => src.MapFrom(p => p.NameArabic))
                .ForMember(dst => dst.Price, src => src.MapFrom(p => p.SellingPrice))
                .ForMember(dst => dst.Quantity, src => src.MapFrom(p => p.Quantity))
                .ForMember(dst => dst.Discount, src => src.MapFrom(p => p.Discount))
                .ForMember(dst => dst.Description, src => src.MapFrom(p => p.Description))
                .ForMember(dst => dst.ImageThumb, src => src.MapFrom(p => p.ImageThumb.ToImageUrl()))
                .ForMember(dst => dst.ImagesGallery, src => src.MapFrom(p => p.ImagesGallery)) //*
                .ForMember(dst => dst.CategoryId, src => src.MapFrom(p => p.CategoryId))
                .ForMember(dst => dst.Highlights, src => src.MapFrom(p => p.ProductHighlights)) //*
                .ForMember(dst => dst.Specifications, src => src.MapFrom(p => p.Specifications)) //*
                .ForMember(dst => dst.Available, src => src.MapFrom(p => p.IsAvailable))
                .ForMember(dst => dst.BrandId, src => src.MapFrom(p => p.Brand.Id))
                .ForMember(dst => dst.BrandCode, src => src.MapFrom(p => p.Brand.Code))
                .ForMember(dst => dst.BrandName, src => src.MapFrom(p => p.Brand.Name))
                //.ForMember(dst => dst.overallRating, src => src.MapFrom(p => p.))
                //.ForMember(dst => dst.reviews, src => src.MapFrom(p => p.SellingPrice))
                .ForMember(dst => dst.SellerId, src => src.MapFrom(p => p.SellerId))
                .ForMember(dst => dst.SellerName, src => src.MapFrom(p => p.Seller.User.FirstName))
                .ForMember(dst => dst.MaxQuantityPerOrder, src => src.MapFrom(p => p.MaxQuantityPerOrder));
            //.ForMember(dst => dst.proCat, src => src.MapFrom(p => p.Category.))
        }
    }
}
