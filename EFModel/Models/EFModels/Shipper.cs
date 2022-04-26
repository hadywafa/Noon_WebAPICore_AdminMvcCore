using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class Shipper
    {
        public string Id { get; set; }

        #region Navigation Propererty

        public ICollection<Order> Orders { get; set; }

        //1-1 Shipper is a user
        public virtual User User { get; set; }

        #endregion

    }
}