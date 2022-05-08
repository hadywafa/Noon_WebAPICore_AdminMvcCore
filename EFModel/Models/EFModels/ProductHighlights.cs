using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFModel.Models.EFModels
{
    public class ProductHighlights :Base
    {
        public string Feature { get; set; }
        //one product have Many Highlights
        public  virtual Product Product { get; set; }
    }
}
