using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.ViewModels.RequestVModels;
using EFModel.Enums;

namespace BL.ViewModels.ResponseVModels
{
    public class VmOrder
    {
        public int Id { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public string DeliveryStatusDescription { get; set; }
        public decimal TotalPrice { get; set; }
        public ICollection<VmOrderItem> Products { get; set; }
    }
}
