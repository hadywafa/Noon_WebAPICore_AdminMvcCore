using EFModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuth.ViewModels
{
    public class OrderVM
    {

        public int orderId { get; set; }
        public DeliveryStatus deliveryStatus { get; set; }

        public decimal totalPrice { get; set; }




    }
}
