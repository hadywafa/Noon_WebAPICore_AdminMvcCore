using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BL.ViewModels.ResponseVModels
{
    [Keyless]
    public class VmCatRoot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameArabic { get; set; }
        public string Description { get; set; }
        public string DescriptionArabic { get; set; }
        public List<VmCatChild> Children { get; set; }
    }
}
