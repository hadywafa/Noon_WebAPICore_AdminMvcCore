using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BL.ViewModels.ResponseVModels;
using EFModel.Models.EFModels;

namespace BL.Profiles
{
    public class OrderItemConfig : Profile
    {
        public OrderItemConfig()
        {
            // target object  => source object
            CreateMap<OrderItem, VmOrderItem>()
                .ForMember(tr => tr.Id, src => src.MapFrom(i => i.Id))
                .ForMember(tr => tr.Product, src => src.MapFrom(i => i.Product))
                .ForMember(tr => tr.Quantity, src => src.MapFrom(i => i.Quantity))
            .ForMember(tr => tr.Price, src => src.MapFrom(i => i.Price));
        }
    }
}
