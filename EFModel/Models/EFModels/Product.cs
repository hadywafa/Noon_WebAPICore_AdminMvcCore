using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class Product : Base
    {
        [Required, MinLength(3), MaxLength(50)]
        public string Name { get; set; }

        [Required, MinLength(3), MaxLength(50)]
        public string NameArabic { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string DescriptionArabic { get; set; }

        [Required]
        public decimal BuyingPrice { get; set; }
        
        public decimal SellingPrice { get; set; }

        public decimal Revenue { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string Weight { get; set; }

        public float Discount { get; set; }

        public bool IsActive { get; set; }

        public DateTime AddedOn { get; set; }

        #region Navigation Property

        // Each Product has a collection of Images
        //[Required]
        public virtual ICollection<Images> Images { get; set; }

        // Each Product is sold by a collection of Sellers
        [ForeignKey("Sellers")]
        public string SellerId { set; get; }
        
        public virtual Seller Seller { get; set; }

        // Each Product has a collection of reviews
        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<Like> Likes { get; set; } 

        public virtual ICollection<Wishlist> Wishlists { get; set; }

        // Each Product can be in many Orders
        public virtual ICollection<OrderItem> Orders { get; set; }

        // many to many between product and cart
        public virtual ICollection<CartProducts> CartProducts { get; set; }

        // Each Product is related to One Category
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
       
        public virtual Category Category { get; set; }
        #endregion

        public Product()
        {
            AddedOn = DateTime.Now;
        }
    }
}
