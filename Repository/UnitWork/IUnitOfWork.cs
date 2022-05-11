using EFModel.Models;
using EFModel.Models.EFModels;
using Repository.CustomRepository.AuthRepo;
using Repository.GenericRepository;
using System;
using System.Threading.Tasks;

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
        IGenericRepo<Admin> Admins { get; }
        IGenericRepo<Customer> Customers { get; }
        IGenericRepo<Seller> Sellers { get; }
        IGenericRepo<Shipper> Shippers { get; }
        IGenericRepo<Brand> Brands { get; }
        IGenericRepo<ProductSpecifications> ProductSpecifications { get; }
        IGenericRepo<ProductHighlights> ProductHighlights { get; }
        IGenericRepo<CustProWishlist> CustProWishlists { get; }
        IGenericRepo<CustProCart> CustProCarts { get; }
        IGenericRepo<CustProSellReviews> Reviews { get; }
        IAuthRepo GetAuthRepo();
        public Task Save();
    }
}
