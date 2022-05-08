using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class Customer
    {
        public Customer()
        {
            createdAt = DateTime.Now;
        }

        public string Id { get; set; }

        public DateTime createdAt { get; set; }

        #region Navigation Propererty

        // Each Customer has a collection of OrderSummaries
        public virtual ICollection<Order> Orders { get; set; }

        //1-1 customer is a user
        public virtual User User { get; set; }

        // 3-ternary relationship Customer review product of specific seller
        public virtual ICollection<CustomerOrderItemSellerReviews> CustomerOrderItemSellerReviews { get; set; }

        // its not mapping its an implementation of [Customer  m => Wishlist <== m Product]
        public virtual ICollection<CustProWishlist> CustProWishlist { get; set; }

        // its not mapping its an implementation of [Customer  m => Cart <== m Product]
        public virtual ICollection<CustProCart> CustProCart { get; set; }

        #endregion


    }
}