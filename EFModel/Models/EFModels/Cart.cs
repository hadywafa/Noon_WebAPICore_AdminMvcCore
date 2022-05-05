using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EFModel.Models.EFModels
{
    public class Cart : Base
    {
        // Each cart is belonged to one customer
        //public int CustomerId { get; set; }
        [Required]
        public Customer Customer { get; set; }

        // Each cart Product
        public virtual ICollection<Product> Products { get; set;}

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public void CalcTotalPrice()
        {
            foreach (var pro in Products)
            {
                TotalPrice += pro.SellingPrice * Quantity;
            }
        }
    }
}
