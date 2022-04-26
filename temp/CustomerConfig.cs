using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFModel.Configurations
{
    public class CustomerConfig :IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> customer)
        {
            customer.HasKey(e => e.Id);
            customer.HasOne<User>(u => u.User).WithOne(s=>s.Customer).HasForeignKey<User>(i => i.Id).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
