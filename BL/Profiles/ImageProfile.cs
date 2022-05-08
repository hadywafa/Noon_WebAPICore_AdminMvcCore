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
    public class ImageProfile : Profile
    {
        public ImageProfile()
        {
            // target object  => source object
            CreateMap<Image, VmImage>()
                .ForMember(tr => tr.Id, src => src.MapFrom(i => i.Id))
                .ForMember(tr => tr.ImageName, src => src.MapFrom(i => i.ImageName.ToImageUrl()));

        }

    }
}
