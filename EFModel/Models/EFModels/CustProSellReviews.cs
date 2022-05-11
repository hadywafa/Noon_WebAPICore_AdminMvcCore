using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EFModel.Enums;

namespace EFModel.Models.EFModels
{
    public class CustProSellReviews 
    {
        public CustProSellReviews()
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

        [MaxLength(200)]
        public string ProductComment { get; set; }

        [MaxLength(200)]
        public string SellerComment { get; set; }

        [Required]
        public bool IsProductCommentAnonymous { get; set; }

        [Required]
        public bool IsSellerCommentAnonymous { get; set; }


        #region Navigation Property

        public virtual Customer Customer { get; set; }
    
        public virtual Seller Seller { get; set; }

        public virtual Product Product { get; set; }

        #endregion

 

    
    }
}
