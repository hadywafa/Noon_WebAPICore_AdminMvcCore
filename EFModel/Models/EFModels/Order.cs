using System;
using System.Collections.Generic;
using EFModel.Enums;

namespace EFModel.Models.EFModels
{
    public class Order : Base
    {
        // Each Order is made by one Customer

        public string CustomerID { get; set; }
        public Customer Customer { get; set; }
        public User User { get; set; }

        //public int ShipperId { get; set; }
        public Shipper Shipper { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        public bool IsPaid { get; set; }

        public decimal Discount { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }


        //DeliveryStatusDescription---Mohamed

        public string DeliveryStatusDescription { get; set; }

        // Each Order has a collection of Items
        public ICollection<OrderItem> OrderItems { get; set; }

        public Order()
        {
            OrderDate = DateTime.Now;
                //foreach (var order in OrderItems)
                //{
                //    TotalPrice += order.Price;
                //}
        }

        public void CalcTotalPrice()
        {
            foreach (var order in OrderItems)
            {
                TotalPrice += order.Price;
            }
        }
    }
}
