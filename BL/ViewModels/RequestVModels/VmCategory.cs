using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.ViewModels.RequestVModels
{
    public class VmCategory
    {
        public int id { get; set; }
        public string Code { get; set; }
        public int Name { get; set; }
        public int CartParentId { get; set; }
    }
}
