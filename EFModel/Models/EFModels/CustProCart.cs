using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFModel.Models.EFModels
{
    public class CustProCart
    {
        public int Quantity { get; set; }

        #region Navigation Property
        
        // its not mapping its an implementation of [Customer  m => Wishlist <== m Product]
        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
        
        #endregion
    }
}
