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
    public class OrderConfig : Profile
    {
        public OrderConfig()
        {
            // target object  => source object
            CreateMap<Order, VmOrder>()
                .ForMember(tr => tr.Id, src => src.MapFrom(i => i.Id))
                .ForMember(tr => tr.DeliveryStatus, src => src.MapFrom(i => i.DeliveryStatus))
                .ForMember(tr => tr.DeliveryStatusDescription, src => src.MapFrom(i => i.DeliveryStatusDescription))
                .ForMember(tr => tr.TotalPrice, src => src.MapFrom(i => i.TotalPrice))
                .ForMember(tr => tr.Products, src => src.MapFrom(i => i.OrderItems));
        }
    }
}
