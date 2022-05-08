using System.Linq;
using EFModel.Configurations;
using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace SqlServerDBContext
{
    public class SqlContext : IdentityDbContext
    {
        public new DbSet<User> Users { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Seller> Sellers { get; set; }
        public virtual DbSet<Shipper> Shippers { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductHighlights> ProductHighlights { get; set; }
        public virtual DbSet<ProductSpecifications> ProductSpecifications { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartProducts> CartProducts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<CustomerProductWishlists> CustomerProductWishlists { get; set; }
        public virtual DbSet<CustomerOrderItemSellerReviews> CustomerOrderItemSellerReviews { get; set; }

        public SqlContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            //Custom Mapping using Fluent Api
            builder.ApplyConfiguration(new UserConfig());
            builder.ApplyConfiguration(new CustOrderItemSellReviewConfig());
            builder.ApplyConfiguration(new CustProWishConfig());

            //Restricted on delete
            foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())
                         .Where(x => x.IsRequired == true && x.IsUnique == true))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}