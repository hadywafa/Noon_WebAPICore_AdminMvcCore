using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFModel.Configurations
{
    public class CustProWishConfig : IEntityTypeConfiguration<CustomerProductWishlists>
    {
        public void Configure(EntityTypeBuilder<CustomerProductWishlists> cps)
        {

            var fkCustomer = "CustomerId";
            var fkProduct = "ProductId";

            cps.HasKey("Id", fkCustomer, fkProduct);

            cps
                .HasOne(fc => fc.Customer)
                .WithMany(x => x.Wishlists)
                .HasForeignKey(fkCustomer);

            cps
                .HasOne(fp => fp.Product)
                .WithMany(y => y.Wishlists)
                .HasForeignKey(fkProduct);

        }
    }

}
