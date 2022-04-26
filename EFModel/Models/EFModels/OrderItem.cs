using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class OrderItem : Base
    {
        // Each OrderItem is related to one order
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        // Each Order is considered as buying a one Product
        [ForeignKey("product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal Price => Product.Price * Quantity;

        //public OrderItem()
        //{
        //    Price = Product.Price * Quantity;
        //}

        //public void CalcPrice()
        //{
        //    Price = Product.Price * Quantity;
        //}
    }
}
