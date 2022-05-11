using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.ViewModels.RequestVModels;
using EFModel.Enums;

namespace BL.ViewModels.ResponseVModels
{
    public class VmOrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public  decimal Price  { get; set; }
        public  VmProduct Product { get; set; }
    }
}
