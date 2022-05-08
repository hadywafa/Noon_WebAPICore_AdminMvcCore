using System.Linq;
using System.Threading.Tasks;
using EFModel.Models;
using EFModel.Models.EFModels;
using Repository.CustomRepository.AuthRepo;
using Repository.GenericRepository;
using SqlServerDBContext;

namespace Repository.UnitWork
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Inject Repositories in Unit of Work
        private readonly SqlContext Context;
        public IAuthRepo AuthRepo { get;  }
        public IGenericRepo<Address> Addresses { get;  }
        public IGenericRepo<Card> Cards { get; }
        public IGenericRepo<Category> Categories { get; }
        public IGenericRepo<User> Users { get;  }
        public IGenericRepo<Image> Images { get;  }
        public IGenericRepo<Order> Orders { get; }
        public IGenericRepo<OrderItem> OrderItems { get;  }
        public IGenericRepo<Product> Products { get;  }
        public IGenericRepo<CustomerOrderItemSellerReviews> Reviews { get;  }
        public IGenericRepo<CustomerProductWishlists> Wishlists { get;  }
        public IGenericRepo<Cart> Carts { get;  }
        public IGenericRepo<Admin> Admins { get; }
        public IGenericRepo<Customer> Customers { get; }
        public IGenericRepo<Seller> Sellers { get; }
        public IGenericRepo<Shipper> Shippers { get; }
        public IGenericRepo<CartProducts> CartProducts { get; }
        public IGenericRepo<Brand> Brands { get; }
        public IGenericRepo<ProductSpecifications> ProductSpecifications { get; }
        public IGenericRepo<ProductHighlights> ProductHighlights { get; }

        // Constructor
        public UnitOfWork(SqlContext _Context, IAuthRepo authRepo)
        {
            Context = _Context;
            Addresses = new GenericRepo<Address>(Context);
            Cards = new GenericRepo<Card>(Context);
            Categories = new GenericRepo<Category>(Context);
            Users = new GenericRepo<User>(Context);
            Images = new GenericRepo<Image>(Context);
            Orders = new GenericRepo<Order>(Context);
            OrderItems = new GenericRepo<OrderItem>(Context);
            Products = new GenericRepo<Product>(Context);
            Reviews = new GenericRepo<CustomerOrderItemSellerReviews>(Context);
            Wishlists = new GenericRepo<CustomerProductWishlists>(Context);
            Carts = new GenericRepo<Cart>(Context);
            Admins = new GenericRepo<Admin>(Context);
            Customers = new GenericRepo<Customer>(Context);
            Sellers = new GenericRepo<Seller>(Context);
            Shippers = new GenericRepo<Shipper>(Context);
            CartProducts = new GenericRepo<CartProducts>(Context);
            Brands = new GenericRepo<Brand>(Context);
            ProductSpecifications = new GenericRepo<ProductSpecifications>(Context);
            ProductHighlights = new GenericRepo<ProductHighlights>(Context);
            AuthRepo = authRepo;
        }
        #endregion

        public async Task  Save()
        {
            await Context.SaveChangesAsync();
        }
        public async void Dispose()
        {
           await Context.DisposeAsync();
        }

        public IAuthRepo GetAuthRepo()
        {
            return AuthRepo;
        }
    }
}