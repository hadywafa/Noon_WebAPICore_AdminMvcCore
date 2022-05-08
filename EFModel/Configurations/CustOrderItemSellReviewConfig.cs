using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFModel.Configurations
{
    public class CustOrderItemSellReviewConfig : IEntityTypeConfiguration<CustomerOrderItemSellerReviews>
    {
        public void Configure(EntityTypeBuilder<CustomerOrderItemSellerReviews> cps)
        {

            var fkCustomer = "CustomerId";
            var fkOrderItem = "OrderItemId";
            var fkSeller = "SellerId";

            cps.HasKey("Id", fkCustomer, fkOrderItem, fkSeller);

            cps
                .HasOne(fc => fc.Customer)
                .WithMany(x => x.CustomerOrderItemSellerReviews)
                .HasForeignKey(fkCustomer);

            cps
                .HasOne(fp => fp.OrderItem)
                .WithMany(y => y.CustomerOrderItemSellerReviews)
                .HasForeignKey(fkOrderItem);

            cps
                .HasOne(fs => fs.Seller)
                .WithMany(z => z.CustomerOrderItemSellerReviews)
                .HasForeignKey(fkSeller);
        }
    }

}
