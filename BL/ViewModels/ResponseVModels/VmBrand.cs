using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFModel.Models.EFModels;

namespace BL.ViewModels.ResponseVModels
{
    public class VmBrand
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsTop { get; set; }
    }
}
