using EFModel.Models;
using EFModel.Models.EFModels;
using Models;
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
        IGenericRepo<Images> Images { get; }
        IGenericRepo<Like> Likes { get; }
        IGenericRepo<Order> Orders { get; }
        IGenericRepo<OrderItem> OrderItems { get; }
        IGenericRepo<Product> Products { get; }
        IGenericRepo<Review> Reviews { get; }
        IGenericRepo<Wishlist> Wishlists { get; }
        IGenericRepo<Cart> Carts { get; }
        IGenericRepo<Admin> Admins { get; }
        IGenericRepo<Customer> Customers { get; }
        IGenericRepo<Seller> Sellers { get; }
        IGenericRepo<Shipper> Shippers { get; }
        IAuthRepo GetAuthRepo();
        public void Save();
    }
}
