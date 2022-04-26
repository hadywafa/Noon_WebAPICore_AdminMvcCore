using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class Images : Base
    {
        public string Image { get; set; }

        // Each Image is belonged to one Product
        [ForeignKey("Product")]

        public virtual int Productid { get; set; }
        public virtual Product Product { get; set; }

        // Each Image is belonged to one Category
        public virtual  Category Category { get; set; }
    }
}