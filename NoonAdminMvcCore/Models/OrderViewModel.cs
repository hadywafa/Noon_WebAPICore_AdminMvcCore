using EFModel.Enums;
using EFModel.Models;
using EFModel.Models.EFModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoonAdminMvcCore.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }
        
        [Required]
        public string DeliveryStatusDescription { get; set; }

        public List<User> Shippers { get; set; }

        public string ShipperId { get; set; }

        public int AddressId { get; set; }

    }
}
