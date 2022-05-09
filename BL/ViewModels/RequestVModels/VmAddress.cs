using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.ViewModels.RequestVModels
{
    public class VmAddress
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public int PostalCode { get; set; }
        public bool IsPrimary { get; set; }
    }
}
