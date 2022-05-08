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
    public class ProductHighlightsProfile : Profile
    {
        public ProductHighlightsProfile()
        {
            CreateMap<ProductHighlights, VmProductHighlights>()
                .ForMember(tr => tr.Feature, src => src.MapFrom(h => h.Feature));
        }
    }
}
