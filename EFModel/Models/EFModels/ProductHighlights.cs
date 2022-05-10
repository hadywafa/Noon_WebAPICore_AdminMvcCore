using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFModel.Models.EFModels
{
    public class ProductHighlights :Base
    {
        [MaxLength(1000)]
        public string Feature { get; set; }
        //one product have Many Highlights
        public  virtual Product Product { get; set; }
    }
}
