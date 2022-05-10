using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFModel.Models.EFModels
{
    public class ProductSpecifications :Base
    {
        [MaxLength(1000)]
        public string Key { get; set; }
        [MaxLength(1000)]
        public string Name { get; set; }
        //each product have many
        public virtual Product Product { get; set; }
    }
}
