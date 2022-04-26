using System.Collections.Generic;

namespace EFModel.Models.EFModels
{
    public class Images : Base
    {
        public string Image { get; set; }

        // Each Image is belonged to one Product
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        // Each Image is belonged to one Category
        public int CategoryId { get; set; }
        public virtual  Category Category { get; set; }
    }
}