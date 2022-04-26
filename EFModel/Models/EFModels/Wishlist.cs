using EFModel.Models;
using EFModel.Models.EFModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Wishlist : Base
    {
        //public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        //public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}
