using EFModel.Models.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFModel.Configurations
{
    public class CustProCartConfig : IEntityTypeConfiguration<CustProCart>
    {
        public void Configure(EntityTypeBuilder<CustProCart> cps)
        {
            var fkCustomer = "CustomerId";
            var fkProduct = "ProductId";

            cps.HasKey( fkCustomer, fkProduct);

            cps
                .HasOne(fc => fc.Customer)
                .WithMany(x => x.CustProCart)
                .HasForeignKey(fkCustomer);

            cps
                .HasOne(fp => fp.Product)
                .WithMany(y => y.CustProCart)
                .HasForeignKey(fkProduct);
        }
    }
}
