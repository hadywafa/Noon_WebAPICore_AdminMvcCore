using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFModel.Models.EFModels
{
    public class ProductSpecifications :Base
    {
        public string Key { get; set; }
        public string Name { get; set; }
        //each product have many
        public virtual Product Product { get; set; }
    }
}
