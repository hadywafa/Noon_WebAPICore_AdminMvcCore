using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.ViewModels.ResponseVModels
{
    public class VmCatChild
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameArabic { get; set; }
        public string Description { get; set; }
        public string DescriptionArabic { get; set; }
        public int ParentId { get; set; }
        public List<VmCatChild> Children { get; set; }
    }
}
