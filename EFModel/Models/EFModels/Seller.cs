using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class Seller
    {
        public Seller()
        {
            createdAt = DateTime.Now;
        }

        public string Id { get; set; }

        public DateTime createdAt { get; set; }

        #region Navigation Propererty

        // Each seller sells a collection of Products
        public virtual ICollection<Product> Products { get; set; }

        //1-1 Seller is a user
        public virtual User User { get; set; }

        // 3-ternary relationship Customer review product of specific seller
        public virtual ICollection<CustProSellReviews> CustProSellReviews { get; set; }
        #endregion



    }
}