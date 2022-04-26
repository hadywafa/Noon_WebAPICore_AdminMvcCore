using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class Admin
    {
        public string Id { get; set; }

        public string Permission { get; set; }

        #region Navigation Propererty

        //1-1 Admin is a user
        public virtual User User { get; set; }

        #endregion
    }
}