using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class Address : Base
    {
        [Required, MinLength(3)]
        public string Street { get; set; }

        [Required, MinLength(3)]
        public string City { get; set; }

        // Egyptian postal code consists of 5 digits
        [Range(10000, 99999)]
        public int PostalCode { get; set; }

        #region Navigation Property
        // Each address is belonged to one User
        public User User { get; set; }

        #endregion

    }
}
