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
        // Each  customer has one cart
        public virtual ICollection<Cart> Cart { get; set; }

        // Each Customer has a collection of OrderSummaries
        public virtual ICollection<Order> Orders { get; set; }
        //User may be customer

        //1-1 customer is a user
        public virtual User User { get; set; }

        // 3-ternary relationship Customer review product of specific seller
        public virtual ICollection<CustomerOrderItemSellerReviews> CustomerOrderItemSellerReviews { get; set; }

        // Each Customer has a collection of Favorite Products
        public virtual ICollection<CustomerProductWishlists> Wishlists { get; set; }

        #endregion


    }
}