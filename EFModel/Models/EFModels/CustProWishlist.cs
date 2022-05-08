using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class CustProWishlist 
    {

        #region Navigation Property
        
        // its not mapping its an implementation of [Customer  m => Wishlist <== m Product]
        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
        
        #endregion
    }
}
