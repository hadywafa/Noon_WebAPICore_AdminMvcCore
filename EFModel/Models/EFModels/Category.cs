using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class Category : Base
    {
        [Required, MaxLength(20)]
        public string Name { get; set; }

        [Required, MaxLength(20)]
        public string NameArabic { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [MaxLength(200)]
        public string DescriptionArabic { get; set; }

        // Each Category has an Image

   
        [Required]
        public Images Image { get; set; }

        #region Navigation Property

        // If Category has a Parent_ID -Which is a self relation to Category ID- it means it's subcategory
        [ForeignKey("Parent")]
        public int? ParentID { get; set; }
        public Category Parent { get; set; }

        // Each Category has a collection of products
        public virtual ICollection<Product> Products { get; set; }
        #endregion
    }
}
