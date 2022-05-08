using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class Product : Base
    {
        public Product()
        {
            AddedOn = DateTime.Now;
        }
        [Required, MinLength(3)]
        public string Name { get; set; }

        [Required, MinLength(3)]
        public string NameArabic { get; set; }

        //[MaxLength(500)]
        public string Description { get; set; }

        //[MaxLength(500)]
        public string DescriptionArabic { get; set; }

        [Required]
        public decimal BuyingPrice { get; set; }
        
        public decimal SellingPrice { get; set; }

        public decimal Revenue { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string Weight { get; set; }

        public float Discount { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime AddedOn { get; set; }


        #region new Props

        //public string SkuId { get; set; }
        //public string SkuString { get; set; }
        public string ModelNumber { get; set; }
        public int WarrantyInDays { get; set; }
        public bool IsFreeDelivered { get; set; }
        public TimeSpan EstimateOrderTime { get; set; }
        public string ImageThumb { get; set; }
        public int MaxQuantityPerOrder { get; set; }
        
        #endregion


        #region Navigation Property

        // Each Product has a collection of ImagesGallery

        // Each Product is sold by a collection of Sellers
        [ForeignKey("Sellers")]
        public string SellerId { set; get; }
        public virtual Seller Seller { get; set; }

        // Each Product is related to One Category
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        // Each Product can be in many Orders
        public virtual ICollection<OrderItem> Orders { get; set; }

        // many to many between product and cart
        public virtual ICollection<CartProducts> CartProducts { get; set; }

        #region new navigation props

        //each product have one brand 
        public virtual Brand Brand { get; set; }

        //each product have multi image gallantry
        public virtual ICollection<Image> ImagesGallery { get; set; }

        // Each Customer has a collection of Favorite Products
        public virtual ICollection<CustomerProductWishlists> Wishlists { get; set; }
        
        //each product have many highlights
        public virtual ICollection<ProductHighlights> ProductHighlights { get; set; }

        //each product have many specifications
        public virtual ICollection<ProductSpecifications> Specifications { get; set; }

        #endregion

        #endregion


    }
}
