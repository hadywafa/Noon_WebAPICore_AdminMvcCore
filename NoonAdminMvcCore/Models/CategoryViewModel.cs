using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoonAdminMvcCore.Models
{
    public class CategoryViewModel
    {
        public string Id { get; set; }

        [Required, MaxLength(20)]
        public string Name { get; set; }

        [Required, MaxLength(20)]
        public string NameArabic { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [MaxLength(200)]
        public string DescriptionArabic { get; set; }

        public IFormFile Image { get; set; }

        public int? ParentId { get; set; }

        //New Data
        [Required, MaxLength(20)]
        public string Code { get; set; }

        public bool IsTop { get; set; }

    }
}
