using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFModel.Configurations
{
    public class UserConfig :IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> user)
        {

            user.Property(u => u.FirstName).IsRequired().HasMaxLength(200);
            user.Property(u => u.LastName).IsRequired().HasMaxLength(200);
            user.HasOne<Admin>(u => u.Admin).WithOne(s => s.User).HasForeignKey<Admin>(i => i.Id);
            user.HasOne<Customer>(u => u.Customer).WithOne(s => s.User).HasForeignKey<Customer>(i => i.Id);
            user.HasOne<Seller>(u => u.Seller).WithOne(s => s.User).HasForeignKey<Seller>(i => i.Id);
            user.HasOne<Shipper>(u => u.Shipper).WithOne(s => s.User).HasForeignKey<Shipper>(i => i.Id);
        }
    }
}
