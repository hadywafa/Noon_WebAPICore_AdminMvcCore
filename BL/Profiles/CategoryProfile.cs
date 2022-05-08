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
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            //Source => target
            //then 
            // tr => src => x
            CreateMap<Category, VmCategory>()
                .ForMember(tr => tr.id, src => src.MapFrom(c => c.Id))
                .ForMember(tr => tr.Name, src => src.MapFrom(c => c.Name))
                .ForMember(tr => tr.Code, src => src.MapFrom(c => c.ParentID));
        }
    }
}
