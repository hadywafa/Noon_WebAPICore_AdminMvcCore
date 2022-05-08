using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EFModel.Enums;

namespace EFModel.Models.EFModels
{
    public class CustomerOrderItemSellerReviews 
    {
        public CustomerOrderItemSellerReviews()
        {
            ReviewDate = DateTime.Now;
        }

        //[Key,Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Rate ProductRate { get; set; }

        [Required]
        public Rate SellerRate { get; set; }

        public DateTime ReviewDate { get; set; }

        [Required]
        public bool IsAsDescription { get; set; }

        [Required]
        public bool IsDeliveredOnTime { get; set; }

        public string ProductComment { get; set; }

        public string SellerComment { get; set; }

        [Required]
        public bool IsProductCommentAnonymous { get; set; }

        [Required]
        public bool IsSellerCommentAnonymous { get; set; }


        #region Navigation Property

        //public string Id { get; set; }
        //[ForeignKey("Id")]
        //[Key, Column(Order = 2)]
        public virtual Customer Customer { get; set; }

        //public string SellerId { get; set; }
        //[ForeignKey("SellerId")]
        //[Key, Column(Order = 3)]
        public virtual Seller Seller { get; set; }

        //public int ProductId { get; set; }
        //[ForeignKey("ProductId")]
        //[Key, Column(Order = 4)]
        public virtual OrderItem OrderItem { get; set; }

        #endregion

 

    
    }
}
