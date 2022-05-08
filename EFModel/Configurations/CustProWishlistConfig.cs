using EFModel.Models.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFModel.Configurations
{
    public class CustProWishlistConfig : IEntityTypeConfiguration<CustProWishlist>
    {
        public void Configure(EntityTypeBuilder<CustProWishlist> cps)
        {
            var fkCustomer = "CustomerId";
            var fkProduct = "ProductId";

            cps.HasKey( fkCustomer, fkProduct);

            cps
                .HasOne(fc => fc.Customer)
                .WithMany(x => x.CustProWishlist)
                .HasForeignKey(fkCustomer);

            cps
                .HasOne(fp => fp.Product)
                .WithMany(y => y.CustProWishlist)
                .HasForeignKey(fkProduct);
        }
    }
}
