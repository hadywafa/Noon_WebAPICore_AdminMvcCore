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
        [Required, MinLength(3), MaxLength(500)]
        public string Name { get; set; }

        [Required, MinLength(3), MaxLength(500)]
        public string NameArabic { get; set; }

        [MaxLength(5000)]
        public string Description { get; set; }

        [MaxLength(5000)]
        public string DescriptionArabic { get; set; }

        public decimal BuyingPrice { get; set; }
        
        public decimal SellingPrice { get; set; }

        public decimal Revenue { get; set; }

        public int Quantity { get; set; }

        public string Weight { get; set; }

        public float Discount { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime AddedOn { get; set; }


        #region new Props
        [MaxLength(500)]
        public string ModelNumber { get; set; }
        public int WarrantyInDays { get; set; }
        public bool IsFreeDelivered { get; set; }
        public TimeSpan EstimateOrderTime { get; set; }
        public string ImageThumb { get; set; }

        [Range(1, 1000)]
        public int MaxQuantityPerOrder { get; set; }
        
        #endregion


        #region Navigation Property

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


        #region new navigation props
        //each product have one brand 
        public virtual Brand Brand { get; set; }

        //each product have multi image gallantry
        public virtual ICollection<Image> ImagesGallery { get; set; }

        // its not mapping its an implementation of [Customer  m => Cart <== m Product]
        public virtual ICollection<CustProCart> CustProCart { get; set; }

        // its not mapping its an implementation of [Customer  m => Wishlist <== m Product]
        public virtual ICollection<CustProWishlist> CustProWishlist { get; set; }
        
        //each product have many highlights
        public virtual ICollection<ProductHighlights> ProductHighlights { get; set; }

        //each product have many specifications
        public virtual ICollection<ProductSpecifications> Specifications { get; set; }

        // 3-ternary relationship Customer review product of specific seller
        public virtual ICollection<CustProSellReviews> CustProSellReviews { get; set; }

        #endregion

        #endregion


    }
}
