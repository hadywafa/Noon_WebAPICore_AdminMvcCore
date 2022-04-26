using System.ComponentModel.DataAnnotations;

namespace EFModel.Models.EFModels
{
    public class Review : Base
    {
        // Each Review is given by one customer
        public int CustomerId { get; set; }
        [Required]
        public Customer Customer { get; set; }

        // Each Review is given for one specific Product
        public int ProductId { get; set; }
        [Required]
        public Product Product { get; set; }

        [Required]
        public decimal Rate { get; set; }
    }
}
