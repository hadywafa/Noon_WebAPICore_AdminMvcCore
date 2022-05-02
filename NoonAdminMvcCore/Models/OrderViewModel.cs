using EFModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoonAdminMvcCore.Models
{
    public class OrderViewModel
    {

        public int Id { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }
        public string DeliveryStatusDescription { get; set; }

    }
}
