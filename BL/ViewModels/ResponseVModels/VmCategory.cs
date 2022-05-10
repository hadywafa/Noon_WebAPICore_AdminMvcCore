using System.Collections.Generic;

namespace BL.ViewModels.ResponseVModels
{
    public class VmCategory
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public int? ParentId { get; set; }
        public ICollection<VmBrand> Brands { get; set; }
        public ICollection<VmCategory> Childrens { get; set; }
    }
}
