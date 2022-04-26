﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModel.Models.EFModels
{
    public class Images : Base
    {
        public string Image { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        // Each Image is belonged to one Category

      
    }
}