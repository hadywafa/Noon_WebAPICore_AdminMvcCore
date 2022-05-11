using EFModel.Enums;
using EFModel.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuth.ViewModels
{
    public class OrderDetails
    {
        public int id { get; set; }

        public Collection<Product> products { get; set; }

        public DeliveryStatus deliveryStatus { get; set; }

        public string deliveryStatusDescription { get; set; }

        public decimal totalPrice { get; set; }

    }
}
