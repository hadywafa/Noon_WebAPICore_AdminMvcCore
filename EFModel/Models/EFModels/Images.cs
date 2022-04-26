using System.Collections.Generic;

namespace EFModel.Models.EFModels
{
    public class Images : Base
    {
        public byte[] Image { get; set; }

        // Each Image is belonged to one Product
        public virtual ICollection<Product> Product { get; set; }

        // Each Image is belonged to one Category
        public virtual ICollection<Category> Category { get; set; }
    }
}