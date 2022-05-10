using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BL.Helpers;
using BL.ViewModels.ResponseVModels;
using EFModel.Models.EFModels;

namespace BL.Profiles
{
    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            // target object  => source object
            CreateMap<Brand, VmBrand>()
                .ForMember(dst => dst.Id, src => src.MapFrom(p => p.Id))
                .ForMember(dst => dst.Code, src => src.MapFrom(p => p.Code))
                .ForMember(dst => dst.Name, src => src.MapFrom(p => p.Name))
                .ForMember(dst => dst.Image, src => src.MapFrom(p => p.Image.ToImageUrl()))
                .ForMember(dst => dst.IsTop, src => src.MapFrom(p => p.IsTop));
        }
    }
}
