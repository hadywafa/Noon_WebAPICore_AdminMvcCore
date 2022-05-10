using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFModel.Models.EFModels
{
    [Index(nameof(Code) , IsUnique = true)]
    public class Brand 
    {
        //[Key,Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //[Key,Column(Order = 2)]
        [Required]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsTop { get; set; }


        #region Navigation Property

        //brand can be in many category
        public virtual ICollection<Product> Products { get; set; }

        //brand can be in many category
        public virtual ICollection<Category> Categories { get; set; }

        #endregion
    }
}
