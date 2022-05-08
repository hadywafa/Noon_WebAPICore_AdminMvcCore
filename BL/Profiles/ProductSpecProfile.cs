using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BL.ViewModels.RequestVModels;
using EFModel.Models.EFModels;

namespace BL.Profiles
{
    public class ProductSpecProfile : Profile
    {
        public ProductSpecProfile()
        {
            CreateMap<ProductSpecifications, VmProductSpec>()
                .ForMember(tr => tr.Key, src => src.MapFrom(s => s.Key))
                .ForMember(tr => tr.Value, src => src.MapFrom(s => s.Name));
        }
    }
}
