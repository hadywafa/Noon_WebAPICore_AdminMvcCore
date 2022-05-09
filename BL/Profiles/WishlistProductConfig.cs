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
    public class WishlistProductConfig :Profile
    {
        public WishlistProductConfig()
        {
            CreateMap<CustProWishlist, VmWishlistProduct>()
                .ForMember(dst => dst.Product, src => src.MapFrom(p => p.Product));
        }
    }
}
