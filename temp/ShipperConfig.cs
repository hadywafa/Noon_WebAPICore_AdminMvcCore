using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFModel.Configurations
{
    public class ShipperConfig :IEntityTypeConfiguration<Shipper>
    {
        public void Configure(EntityTypeBuilder<Shipper> shipper)
        {
            shipper.HasKey(e => e.Id);
            shipper.HasOne<User>(u => u.User).WithOne(s=>s.Shipper).HasForeignKey<User>(i => i.Id).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
