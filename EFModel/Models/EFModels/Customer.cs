using Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class Customer
    {
        public string Id { get; set; }

        #region Navigation Propererty

        // Each  customer has one cart
        public virtual ICollection<Cart> Cart { get; set; }

        // Each customer has a collection of Reviews
        public virtual ICollection<Review> Reviews { get; set; }

        // Each Customer has a collection of Wish list
        public virtual ICollection<Wishlist> Wishlists { get; set; }

        // Each Customer has a collection of Likes
        public virtual ICollection<Like> Likes { get; set; }

        // Each Customer has a collection of OrderSummaries
        public virtual ICollection<Order> Orders { get; set; }
        //User may be customer

        //1-1 customer is a user
        public virtual User User { get; set; }


        #endregion


    }
}