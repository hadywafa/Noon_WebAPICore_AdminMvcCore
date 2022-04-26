using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFModel.Configurations
{
    public class AdminConfig :IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> admin)
        {
            admin.HasKey(e => e.Id);
            admin.HasOne<User>(u => u.User).WithOne(s=>s.Admin).HasForeignKey<User>(i => i.Id).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
