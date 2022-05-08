using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class CustomerProductWishlists 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        #region Navigation Property

        //public string CustomerId { get; set; }
        //[ForeignKey("CustomerId")]
        //[Key, Column(Order = 1)]
        public Customer Customer { get; set; }

        //public int ProductId { get; set; }
        //[ForeignKey("ProductId")]
        //[Key, Column(Order = 2)] //< Not Working
        public Product Product { get; set; }
        
        #endregion

    }
}
