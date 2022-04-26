using EFModel.Configurations;
using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace SqlServerDBContext
{
    public class SqlContext : IdentityDbContext
    {
        public new DbSet<User> Users { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Seller> Sellers { get; set; }
        public virtual DbSet<Shipper> Shippers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Images> Images { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }

        public SqlContext(DbContextOptions options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserConfig());
            //builder.ApplyConfiguration(new AdminConfig());
            //builder.ApplyConfiguration(new CustomerConfig());
            //builder.ApplyConfiguration(new SellerConfig());
            //builder.ApplyConfiguration(new ShipperConfig());
        }

    }
}
