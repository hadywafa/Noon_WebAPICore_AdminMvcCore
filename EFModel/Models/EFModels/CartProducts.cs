using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EFModel.Models.EFModels
{
    public class CartProducts :Base
    {
        //navigation properties
        public virtual Product Products { get; set;}
        public virtual Cart Cart { get; set;}
        //property on relation
        public int Quantity { get; set; }
    }
}
