using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFModel.Configurations
{
    public class SellerConfig :IEntityTypeConfiguration<Seller>
    {
        public void Configure(EntityTypeBuilder<Seller> seller)
        {
            seller.HasKey(e => e.Id);
            seller.HasOne<User>(u => u.User).WithOne(s=>s.Seller).HasForeignKey<User>(i => i.Id).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
