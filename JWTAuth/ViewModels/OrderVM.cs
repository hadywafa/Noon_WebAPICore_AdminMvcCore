using EFModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuth.ViewModels
{
    public class OrderVM
    {

        public int OrderId { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }

        public decimal TotalPrice { get; set; }




    }
}
