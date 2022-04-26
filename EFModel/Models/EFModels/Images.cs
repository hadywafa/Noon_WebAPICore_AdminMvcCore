using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class Images : Base
    {
        public string Image { get; set; }

        // Each Image is belonged to one Product
<<<<<<< HEAD
        [ForeignKey("Product")]

        public virtual int Productid { get; set; }
=======
        public int ProductId { get; set; }
>>>>>>> ddddcad7cedcb2f205b16c64e8653babf6c4bdba
        public virtual Product Product { get; set; }

        // Each Image is belonged to one Category
        public int CategoryId { get; set; }
        public virtual  Category Category { get; set; }
    }
}