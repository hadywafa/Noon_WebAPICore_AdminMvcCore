using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BL.ViewModels.RequestVModels;
using BL.ViewModels.ResponseVModels;
using EFModel.Models.EFModels;

namespace BL.Profiles
{
    public class CartProductProfile : Profile
    {
        public CartProductProfile()
        {
            CreateMap<CustProCart, VmCartProduct>()
                .ForMember(tr => tr.Quantity, src => src.MapFrom(c => c.Quantity))
                .ForMember(tr => tr.Product, src => src.MapFrom(c => c.Product));
        }
    }
}
