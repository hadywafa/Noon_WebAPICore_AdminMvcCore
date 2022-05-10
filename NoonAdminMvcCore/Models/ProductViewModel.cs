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

        [Required(ErrorMessage = "Name of product is required"), MinLength(3), MaxLength(150)]
        public string Name { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب"), MinLength(3), MaxLength(150)]
        public string NameArabic { get; set; }

        public string ModelNumber { get; set; }

        [MaxLength(5000)]
        public string Description { get; set; }

        [MaxLength(5000)]
        public string DescriptionArabic { get; set; }

        [Required(ErrorMessage = "price of product is required")]
        public decimal BuyingPrice { get; set; }

        [Required(ErrorMessage = "price of product is required")]
        public decimal SellingPrice { get; set; }

        [Required(ErrorMessage = "Quantity of product is required")]
        public int Quantity { get; set; }

        public string Weight { get; set; }

        [Required(ErrorMessage = "Discount of product is required")]
        public float Discount { get; set; }

        public bool IsActive { get; set; }

        [Range(1, 1000)]
        public int MaxQuantityPerOrder { get; set; }

        public bool isAvaliable { get; set; }


        // Each Product has a collection of ImagesGallery
        public IFormFile[] Images { get; set; }

        // Each Product is sold by a collection of Sellers
        public string SellerId { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
