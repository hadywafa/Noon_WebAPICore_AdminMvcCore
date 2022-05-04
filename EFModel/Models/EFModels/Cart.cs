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
        //public int ProductID { get; set; }
        public Product Product { get; set;}

        public int Quantity { get; set; }
        public decimal Price => Product.SellingPrice * Quantity;

    }
}
