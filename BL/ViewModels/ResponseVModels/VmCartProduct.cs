using BL.ViewModels.RequestVModels;

namespace BL.ViewModels.ResponseVModels
{
    public class VmCartProduct
    {
        public int Quantity { get; set; }
        public VmProduct Product { get; set; }
    }
}
