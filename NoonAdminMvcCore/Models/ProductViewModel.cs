using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoonAdminMvcCore.Models
{
    public class ProductViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Name of product is required"), MinLength(3), MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب*"), MinLength(3), MaxLength(50)]
        public string NameArabic { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string DescriptionArabic { get; set; }

        [Required(ErrorMessage = "price of product is required")]
        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }

        [Required(ErrorMessage = "price of product is required")]
        public int Quantity { get; set; }

        public string Weight { get; set; }

        public float Discount { get; set; }

        public bool IsActive { get; set; }


        // Each Product has a collection of Images
        [Required]
        public IFormFile[] Images { get; set; }

        // Each Product is sold by a collection of Sellers
        public string SellerId { get; set; }
        public int CategoryId { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
