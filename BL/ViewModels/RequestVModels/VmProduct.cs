using System.Collections.Generic;
using EFModel.Models.EFModels;

namespace BL.ViewModels.RequestVModels
{
    public class VmProduct
    {
        //All nested Object Must be  => VM
        //public string skuId { get; set; }
        //public string skuString { get; set; }
        public int Id { get; set; }
        public string ModelNumber { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public float Discount { get; set; }
        public string Description { get; set; }
        public string ImageThumb { get; set; }
        public ICollection<VmImage> ImagesGallery { get; set; }
        public int CategoryId { get; set; }
        public ICollection<VmProductHighlights> Highlights { get; set; }
        public ICollection<VmProductSpec> Specifications { get; set; }
        public bool Available { get; set; }
        public int BrandId { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        //public int OverallRating { get; set; }
        //public VmReview Reviews { get; set; }//will not mapping
        public string SellerId { get; set; }
        public string SellerName { get; set; }
        public int MaxQuantityPerOrder { get; set; }
        public bool IsFreeDelivered { get; set; }
        
    }
}
