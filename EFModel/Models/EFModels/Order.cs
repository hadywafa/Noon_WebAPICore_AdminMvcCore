using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EFModel.Enums;

namespace EFModel.Models.EFModels
{
    public class Order : Base
    {
        // Each Order is made by one Customer

        public string CustomerID { get; set; }
        public Customer Customer { get; set; }
        public User User { get; set; }

        public string ShipperId { get; set; }
        public Shipper Shipper { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal Discount { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }

        public PaymentMethod PaymentMethod { get; set; }


        //DeliveryStatusDescription---Mohamed
        public string DeliveryStatusDescription { get; set; }

        // Each Order has a collection of Items
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        [ForeignKey("Address")]
        public int AddressId { get; set; }
        public virtual Address Address { get; set; }

        public Order()
        {
            OrderDate = DateTime.Now;
            DeliveryStatus = DeliveryStatus.Processing;
            DeliveryStatusDescription = "Your Orders is placed, awaiting for shipping";
        }

        public void CalcTotalPrice()
        {
            foreach (var order in OrderItems)
            {
                TotalPrice += (order.Price-(order.Price * Discount));
                TotalRevenue += order.Revenue;
            }
        }
    }
}
