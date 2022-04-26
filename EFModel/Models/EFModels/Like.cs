using System.ComponentModel.DataAnnotations;

namespace EFModel.Models.EFModels
{
    public class Like : Base
    {
        #region Navigation Property
        //public int CustomerId { get; set; }
        [Required]
        public virtual Customer Customer { get; set; }

        //public int ProductId { get; set; }
        [Required]
        public virtual Product Product { get; set; }

        #endregion
    }
}
