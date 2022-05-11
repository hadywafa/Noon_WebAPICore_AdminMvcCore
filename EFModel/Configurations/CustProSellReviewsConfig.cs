using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFModel.Configurations
{
    public class CustProSellReviewsConfig : IEntityTypeConfiguration<CustProSellReviews>
    {
        public void Configure(EntityTypeBuilder<CustProSellReviews> cps)
        {

            var fkCustomer = "CustomerId";
            var fkProduct = "ProductId";
            var fkSeller = "SellerId";

            cps.HasKey("Id", fkCustomer, fkProduct, fkSeller);

            cps
                .HasOne(fc => fc.Customer)
                .WithMany(x => x.CustProSellReviews)
                .HasForeignKey(fkCustomer);

            cps
                .HasOne(fp => fp.Product)
                .WithMany(y => y.CustProSellReviews)
                .HasForeignKey(fkProduct);

            cps
                .HasOne(fs => fs.Seller)
                .WithMany(z => z.CustProSellReviews)
                .HasForeignKey(fkSeller);
        }
    }

}
