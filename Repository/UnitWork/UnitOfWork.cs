using System.Linq;
using EFModel.Models;
using EFModel.Models.EFModels;
using Models;
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
        public IGenericRepo<Images> Images { get;  }
        public IGenericRepo<Like> Likes { get;  }
        public IGenericRepo<Order> Orders { get; }
        public IGenericRepo<OrderItem> OrderItems { get;  }
        public IGenericRepo<Product> Products { get;  }
        public IGenericRepo<Review> Reviews { get;  }
        public IGenericRepo<Wishlist> Wishlists { get;  }
        public IGenericRepo<Cart> Carts { get;  }
        public IGenericRepo<Admin> Admins { get; }
        public IGenericRepo<Customer> Customers { get; }
        public IGenericRepo<Seller> Sellers { get; }
        public IGenericRepo<Shipper> Shippers { get; }
        public IGenericRepo<CartProducts> CartProducts { get; }

        // Constructor
        public UnitOfWork(SqlContext _Context, IAuthRepo authRepo)
        {
            Context = _Context;
            Addresses = new GenericRepo<Address>(Context);
            Cards = new GenericRepo<Card>(Context);
            Categories = new GenericRepo<Category>(Context);
            Users = new GenericRepo<User>(Context);
            Images = new GenericRepo<Images>(Context);
            Likes = new GenericRepo<Like>(Context);
            Orders = new GenericRepo<Order>(Context);
            OrderItems = new GenericRepo<OrderItem>(Context);
            Products = new GenericRepo<Product>(Context);
            Reviews = new GenericRepo<Review>(Context);
            Wishlists = new GenericRepo<Wishlist>(Context);
            Carts = new GenericRepo<Cart>(Context);
            Admins = new GenericRepo<Admin>(Context);
            Customers = new GenericRepo<Customer>(Context);
            Sellers = new GenericRepo<Seller>(Context);
            Shippers = new GenericRepo<Shipper>(Context);
            CartProducts = new GenericRepo<CartProducts>(Context);
            AuthRepo = authRepo;
        }
        #endregion

        public void Save()
        {
            Context.SaveChanges();
        }
        public void Dispose()
        {
            Context.Dispose();
        }

        public IAuthRepo GetAuthRepo()
        {
            return AuthRepo;
        }
    }
}