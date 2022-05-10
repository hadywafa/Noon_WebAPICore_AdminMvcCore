using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModel.Models.EFModels
{
    [Index(nameof(Code) , IsUnique = true)]
    public class Category 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string Code { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(200)]
        public string NameArabic { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [MaxLength(200)]
        public string DescriptionArabic { get; set; }

        public Image Image { get; set; }

        public bool IsTop { get; set; }

        #region Navigation Property

        // If Category has a Parent_ID -Which is a self relation to Category ID- it means it's subcategory
        [ForeignKey("Parent")]
        public int? ParentID { get; set; }
        public virtual Category Parent { get; set; }

        // Each Category has a collection of products
        public virtual ICollection<Product> Products { get; set; }

        //each category can have many brand
        public virtual ICollection<Brand> Brands { get; set; }
        #endregion
    }
}