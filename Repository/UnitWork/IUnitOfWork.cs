using EFModel.Models;
using EFModel.Models.EFModels;
using Repository.CustomRepository.AuthRepo;
using Repository.GenericRepository;
using System;

namespace Repository.UnitWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepo<Address> Addresses { get; }
        IGenericRepo<Card> Cards { get; }
        IGenericRepo<Category> Categories { get; }
        IGenericRepo<User> Users { get; }
        IGenericRepo<Image> Images { get; }
        IGenericRepo<Order> Orders { get; }
        IGenericRepo<OrderItem> OrderItems { get; }
        IGenericRepo<Product> Products { get; }
        IGenericRepo<CustomerOrderItemSellerReviews> Reviews { get; }
        IGenericRepo<CustomerProductWishlists> Wishlists { get; }
        IGenericRepo<Cart> Carts { get; }
        IGenericRepo<Admin> Admins { get; }
        IGenericRepo<Customer> Customers { get; }
        IGenericRepo<Seller> Sellers { get; }
        IGenericRepo<Shipper> Shippers { get; }
        IGenericRepo<CartProducts> CartProducts { get; }
        IGenericRepo<Brand> Brands { get; }
        IGenericRepo<ProductSpecifications> ProductSpecifications { get; }
        IGenericRepo<ProductHighlights> ProductHighlights { get; }
        IAuthRepo GetAuthRepo();
        public void Save();
    }
}
