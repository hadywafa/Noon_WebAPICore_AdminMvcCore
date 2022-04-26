using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BL.AppPolicy;
using EFModel.Enums;
using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Identity;
using Repository.GenericRepository;
using Repository.UnitWork;

namespace NoonAdminMvcCore.Controllers
{
    public class HwController : Controller
    {
        #region Inject Dependencies

        // Unit Of Work which is responsible on operations on Context
        private readonly IUnitOfWork _unitOfWork;

        // User Repo which is responsible on operations on user
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        readonly IGenericRepo<User> _userRepo;
        readonly IGenericRepo<Address> _addressRepo;
        readonly IGenericRepo<Category> _catRepo;
        readonly IGenericRepo<Product> _productRepo;
        readonly IGenericRepo<OrderItem> _orderItemsRepo;
        readonly IGenericRepo<Order> _orderRepo;
        readonly IGenericRepo<Admin> _adminRepo;
        readonly IGenericRepo<Customer> _customerRepo;
        readonly IGenericRepo<Seller> _sellerRepo;
        readonly IGenericRepo<Shipper> _shipperRepo;

        // Constructor
        public HwController(IUnitOfWork unitOfWork, UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepo = _unitOfWork.Users;
            _addressRepo = _unitOfWork.Addresses;
            _catRepo = _unitOfWork.Categories;
            _productRepo = _unitOfWork.Products;
            _orderItemsRepo = _unitOfWork.OrderItems;
            _orderRepo = _unitOfWork.Orders;
            _adminRepo = _unitOfWork.Admins;
            _customerRepo = _unitOfWork.Customers;
            _sellerRepo = _unitOfWork.Sellers;
            _shipperRepo = _unitOfWork.Shippers;
        }

        #endregion

        public async Task<string> HwAddRoles()
        {
            var team4 = new List<string>() { "Mo", "Kero", "Emad", "Hady" };
            var roles = new List<string>()
            {
                AuthorizeRoles.Admin, AuthorizeRoles.Customer, AuthorizeRoles.Seller, AuthorizeRoles.Shipper
            };
            bool isInserted = _roleManager.Roles.Any(r =>
                r.Name == AuthorizeRoles.Admin || r.Name == AuthorizeRoles.Customer ||
                r.Name == AuthorizeRoles.Seller || r.Name == AuthorizeRoles.Shipper);
            if (!isInserted)
            {
                await _roleManager.CreateAsync(new IdentityRole(AuthorizeRoles.Admin));
                await _roleManager.CreateAsync(new IdentityRole(AuthorizeRoles.Customer));
                await _roleManager.CreateAsync(new IdentityRole(AuthorizeRoles.Seller));
                await _roleManager.CreateAsync(new IdentityRole(AuthorizeRoles.Shipper));
                return "Seeding Roles have been Added Successfully";
            }

            return "there are an Error or Seeding Roles are Existed";
        }

        public async Task<string> HwAddUsers()
        {
            var team4 = new List<string>() { "Mo", "Kero", "Emad", "Hady" };
            var roles = new List<string>()
            {
                AuthorizeRoles.Admin, AuthorizeRoles.Customer, AuthorizeRoles.Seller, AuthorizeRoles.Shipper
            };
            bool isInserted = _userRepo.GetAll()
                .Any(u => u.FirstName == "Mo" || u.FirstName == "Kero" || u.FirstName == "Emad" ||
                          u.FirstName == "Hady");
            if (!isInserted)
            {
                foreach (string name in team4)
                {
                    foreach (var role in roles)
                    {
                        var user = new User { FirstName = name, LastName = role, Balance = 5000, IsActive = true , PhoneNumber = "0123456789"};
                        await _userManager.AddPasswordAsync(user, $"{name}@1234");
                        await _userManager.SetEmailAsync(user, $"{user.FirstName + user.LastName}@gmail.com");
                        await _userManager.AddToRoleAsync(user, role);
                        _userRepo.Add(user);
                        switch (role)
                        {
                            case AuthorizeRoles.Admin:
                                _adminRepo.Add(new Admin() { User = user });
                                break;
                            case AuthorizeRoles.Customer:
                                _customerRepo.Add(new Customer() { User = user });
                                break;
                            case AuthorizeRoles.Seller:
                                _sellerRepo.Add(new Seller() { User = user });
                                break;
                            case AuthorizeRoles.Shipper:
                                _shipperRepo.Add(new Shipper() { User = user });
                                break;
                        }

                        _unitOfWork.Save();
                        var address = new Address
                        {
                            User = user,
                            Street = "Fun",
                            City = $"city{user.FirstName + user.LastName}",
                            PostalCode = 1234
                        };
                        _addressRepo.Add(address);
                        _unitOfWork.Save();
                    }
                }

                return "dummy Users have been Added Successfully";
            }

            return "There an Error or dummy Users are existed";
        }

        public string HwAddCategory()
        {
            bool isInserted = _catRepo.GetAll().Any(c => c.Name.StartsWith("Category"));
            if (!isInserted)
            {
                for (int i = 0; i <= 10; i++)
                {
                    var cat = new Category()
                    {
                        Name = $"Category {i}",
                        NameArabic = $"صنف{i}",
                        Description = "HHHHHHHHHHH",
                        DescriptionArabic = "هههههههههههههههه",
                        Image = new Images() { Image = new byte[5] },
                    };
                    _catRepo.Add(cat);
                    _unitOfWork.Save();
                }

                return "dummy Categories have been Added Successfully";
            }

            return "there are an Error or dummy Categories are Existed";
        }

        public string HwAddProducts()
        {
            bool isInserted = _productRepo.GetAll().Any(c => c.Name.StartsWith("Product"));
            if (!isInserted)
            {
                for (int i = 1; i <= 10; i++)
                {
                    var pro = new Product()
                    {
                        Name = $"Product {i}",
                        NameArabic = $"منتج{i}",
                        Description = "HHHHHHHHHHH",
                        DescriptionArabic = "هههههههههههههههه",
                        Category = _catRepo.Find(c => c.Name == "Category 1"),
                        Price = 50 * (i + 1),
                        Quantity = 2 * (i + 1),
                        Discount = 0.2f,
                        IsActive = true,
                        AddedOn = DateTime.Now,
                        Weight = $"{5 * (i + 1)} kg",
                        Sellers = _sellerRepo.Find(s => s.User.Email == "MoSeller@gmail.com"),
                    };
                    _productRepo.Add(pro);
                    _unitOfWork.Save();
                }

                for (int i = 11; i <= 20; i++)
                {
                    var pro = new Product()
                    {
                        Name = $"Product {i}",
                        NameArabic = $"منتج{i}",
                        Description = "HHHHHHHHHHH",
                        DescriptionArabic = "هههههههههههههههه",
                        Category = _catRepo.Find(c => c.Name == "Category 1"),
                        Price = 50 * (i + 1),
                        Quantity = 2 * (i + 1),
                        Discount = 0.2f,
                        IsActive = true,
                        AddedOn = DateTime.Now,
                        Weight = $"{5 * (i + 1)} kg",
                        Sellers = _sellerRepo.Find(s => s.User.Email == "EmadSeller@gmail.com"),
                    };
                    _productRepo.Add(pro);
                    _unitOfWork.Save();
                }

                for (int i = 21; i <= 30; i++)
                {
                    var pro = new Product()
                    {
                        Name = $"Product {i}",
                        NameArabic = $"منتج{i}",
                        Description = "HHHHHHHHHHH",
                        DescriptionArabic = "هههههههههههههههه",
                        Category = _catRepo.Find(c => c.Name == "Category 1"),
                        Price = 50 * (i + 1),
                        Quantity = 2 * (i + 1),
                        Discount = 0.2f,
                        IsActive = true,
                        AddedOn = DateTime.Now,
                        Weight = $"{5 * (i + 1)} kg",
                        Sellers = _sellerRepo.Find(s => s.User.Email == "KeroSeller@gmail.com"),
                    };
                    _productRepo.Add(pro);
                    _unitOfWork.Save();
                }

                for (int i = 31; i <= 40; i++)
                {
                    var pro = new Product()
                    {
                        Name = $"Product {i}",
                        NameArabic = $"منتج{i}",
                        Description = "HHHHHHHHHHH",
                        DescriptionArabic = "هههههههههههههههه",
                        Category = _catRepo.Find(c => c.Name == "Category 1"),
                        Price = 50 * (i + 1),
                        Quantity = 2 * (i + 1),
                        Discount = 0.2f,
                        IsActive = true,
                        AddedOn = DateTime.Now,
                        Weight = $"{5 * (i + 1)} kg",
                        Sellers = _sellerRepo.Find(s => s.User.Email == "HadySeller@gmail.com"),
                    };
                    _productRepo.Add(pro);
                    _unitOfWork.Save();
                }

                return "dummy Products have been Added Successfully";
            }

            return "there are an Error or dummy Products are Existed";
        }

        public string HwAddOrder()
        {
            bool isInserted = _orderRepo.GetAll().Any(c => c.Discount == (decimal)0.5);
            if (!isInserted)
            {
                #region Mo Orders

                var orderMo1 = new Order()
                {
                    Customer = _customerRepo.Find(c => c.User.Email == "MoCustomer@gmail.com"),
                    Shipper = _shipperRepo.Find(s => s.User.Email == "MoShipper@gmail.com"),
                    Discount = (decimal)0.5,
                    IsPaid = false,
                    DeliveryStatus = DeliveryStatus.Processing,
                    OrderDate = DateTime.Now,
                    OrderItems = new Collection<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Quantity = 15, Product = _productRepo.Find(p => p.Name == "Product 1")
                        },
                        new OrderItem()
                        {
                            Quantity = 42, Product = _productRepo.Find(p => p.Name == "Product 2")
                        },
                        new OrderItem()
                        {
                            Quantity = 14, Product = _productRepo.Find(p => p.Name == "Product 3")
                        },
                        new OrderItem()
                        {
                            Quantity = 32, Product = _productRepo.Find(p => p.Name == "Product 4")
                        },
                        new OrderItem()
                        {
                            Quantity = 33, Product = _productRepo.Find(p => p.Name == "Product 5")
                        },
                    }
                };
                orderMo1.CalcTotalPrice();
                _orderRepo.Add(orderMo1);

                var orderMo2 = new Order()
                {
                    Customer = _customerRepo.Find(c => c.User.Email == "MoCustomer@gmail.com"),
                    Shipper = _shipperRepo.Find(s => s.User.Email == "MoShipper@gmail.com"),
                    Discount = (decimal)0.5,
                    IsPaid = false,
                    DeliveryStatus = DeliveryStatus.Processing,
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Quantity = 23, Product = _productRepo.Find(p => p.Name == "Product 6")
                        },
                        new OrderItem()
                        {
                            Quantity = 123, Product = _productRepo.Find(p => p.Name == "Product 7")
                        },
                        new OrderItem()
                        {
                            Quantity = 41, Product = _productRepo.Find(p => p.Name == "Product 8")
                        },
                        new OrderItem()
                        {
                            Quantity = 233, Product = _productRepo.Find(p => p.Name == "Product 9")
                        },
                        new OrderItem()
                        {
                            Quantity = 115, Product = _productRepo.Find(p => p.Name == "Product 10")
                        },
                    }
                };

                orderMo2.CalcTotalPrice();
                _orderRepo.Add(orderMo2);

                #endregion

                #region Kero Orders

                var orderKero1 = new Order()
                {
                    Customer = _customerRepo.Find(c => c.User.Email == "KeroCustomer@gmail.com"),
                    Shipper = _shipperRepo.Find(s => s.User.Email == "KeroShipper@gmail.com"),
                    Discount = (decimal)0.5,
                    IsPaid = false,
                    DeliveryStatus = DeliveryStatus.Processing,
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Quantity = 15, Product = _productRepo.Find(p => p.Name == "Product 1")
                        },
                        new OrderItem()
                        {
                            Quantity = 42, Product = _productRepo.Find(p => p.Name == "Product 2")
                        },
                        new OrderItem()
                        {
                            Quantity = 14, Product = _productRepo.Find(p => p.Name == "Product 3")
                        },
                        new OrderItem()
                        {
                            Quantity = 32, Product = _productRepo.Find(p => p.Name == "Product 4")
                        },
                        new OrderItem()
                        {
                            Quantity = 33, Product = _productRepo.Find(p => p.Name == "Product 5")
                        },
                    }
                };
                orderKero1.CalcTotalPrice();
                _orderRepo.Add(orderKero1);

                var orderKero2 = new Order()
                {
                    Customer = _customerRepo.Find(c => c.User.Email == "KeroCustomer@gmail.com"),
                    Shipper = _shipperRepo.Find(s => s.User.Email == "KeroShipper@gmail.com"),
                    Discount = (decimal)0.5,
                    IsPaid = false,
                    DeliveryStatus = DeliveryStatus.Processing,
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Quantity = 23, Product = _productRepo.Find(p => p.Name == "Product 6")
                        },
                        new OrderItem()
                        {
                            Quantity = 123, Product = _productRepo.Find(p => p.Name == "Product 7")
                        },
                        new OrderItem()
                        {
                            Quantity = 41, Product = _productRepo.Find(p => p.Name == "Product 8")
                        },
                        new OrderItem()
                        {
                            Quantity = 233, Product = _productRepo.Find(p => p.Name == "Product 9")
                        },
                        new OrderItem()
                        {
                            Quantity = 115, Product = _productRepo.Find(p => p.Name == "Product 10")
                        },
                    }
                };
                orderKero2.CalcTotalPrice();
                _orderRepo.Add(orderKero2);

                #endregion

                #region Emad Orders

                var orderEmad1 = new Order()
                {
                    Customer = _customerRepo.Find(c => c.User.Email == "EmadCustomer@gmail.com"),
                    Shipper = _shipperRepo.Find(s => s.User.Email == "EmadShipper@gmail.com"),
                    Discount = (decimal)0.5,
                    IsPaid = false,
                    DeliveryStatus = DeliveryStatus.Processing,
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Quantity = 15, Product = _productRepo.Find(p => p.Name == "Product 1")
                        },
                        new OrderItem()
                        {
                            Quantity = 42, Product = _productRepo.Find(p => p.Name == "Product 2")
                        },
                        new OrderItem()
                        {
                            Quantity = 14, Product = _productRepo.Find(p => p.Name == "Product 3")
                        },
                        new OrderItem()
                        {
                            Quantity = 32, Product = _productRepo.Find(p => p.Name == "Product 4")
                        },
                        new OrderItem()
                        {
                            Quantity = 33, Product = _productRepo.Find(p => p.Name == "Product 5")
                        },
                    }
                };
                orderEmad1.CalcTotalPrice();
                _orderRepo.Add(orderEmad1);

                var orderEmad2 = new Order()
                {
                    Customer = _customerRepo.Find(c => c.User.Email == "EmadCustomer@gmail.com"),
                    Shipper = _shipperRepo.Find(s => s.User.Email == "EmadShipper@gmail.com"),
                    Discount = (decimal)0.5,
                    IsPaid = false,
                    DeliveryStatus = DeliveryStatus.Processing,
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Quantity = 23, Product = _productRepo.Find(p => p.Name == "Product 6")
                        },
                        new OrderItem()
                        {
                            Quantity = 123, Product = _productRepo.Find(p => p.Name == "Product 7")
                        },
                        new OrderItem()
                        {
                            Quantity = 41, Product = _productRepo.Find(p => p.Name == "Product 8")
                        },
                        new OrderItem()
                        {
                            Quantity = 233, Product = _productRepo.Find(p => p.Name == "Product 9")
                        },
                        new OrderItem()
                        {
                            Quantity = 115, Product = _productRepo.Find(p => p.Name == "Product 10")
                        },
                    }
                };
                orderEmad2.CalcTotalPrice();
                _orderRepo.Add(orderEmad2);

                #endregion

                #region Hady Orders

                var orderHady1 = new Order()
                {
                    Customer = _customerRepo.Find(c => c.User.Email == "HadyCustomer@gmail.com"),
                    Shipper = _shipperRepo.Find(s => s.User.Email == "HadyShipper@gmail.com"),
                    Discount = (decimal)0.5,
                    IsPaid = false,
                    DeliveryStatus = DeliveryStatus.Processing,
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Quantity = 15, Product = _productRepo.Find(p => p.Name == "Product 1")
                        },
                        new OrderItem()
                        {
                            Quantity = 42, Product = _productRepo.Find(p => p.Name == "Product 2")
                        },
                        new OrderItem()
                        {
                            Quantity = 14, Product = _productRepo.Find(p => p.Name == "Product 3")
                        },
                        new OrderItem()
                        {
                            Quantity = 32, Product = _productRepo.Find(p => p.Name == "Product 4")
                        },
                        new OrderItem()
                        {
                            Quantity = 33, Product = _productRepo.Find(p => p.Name == "Product 5")
                        },
                    }
                };
                orderHady1.CalcTotalPrice();
                _orderRepo.Add(orderHady1);

                var orderHady2 = new Order()
                {
                    Customer = _customerRepo.Find(c => c.User.Email == "HadyCustomer@gmail.com"),
                    Shipper = _shipperRepo.Find(s => s.User.Email == "HadyShipper@gmail.com"),
                    Discount = (decimal)0.5,
                    IsPaid = false,
                    DeliveryStatus = DeliveryStatus.Processing,
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Quantity = 23, Product = _productRepo.Find(p => p.Name == "Product 6")
                        },
                        new OrderItem()
                        {
                            Quantity = 123, Product = _productRepo.Find(p => p.Name == "Product 7")
                        },
                        new OrderItem()
                        {
                            Quantity = 41, Product = _productRepo.Find(p => p.Name == "Product 8")
                        },
                        new OrderItem()
                        {
                            Quantity = 233, Product = _productRepo.Find(p => p.Name == "Product 9")
                        },
                        new OrderItem()
                        {
                            Quantity = 115, Product = _productRepo.Find(p => p.Name == "Product 10")
                        },
                    }
                };
                orderHady2.CalcTotalPrice();
                _orderRepo.Add(orderHady2);

                #endregion

                _unitOfWork.Save();

                return "dummy Orders have been Added Successfully";
            }

            return "there are an Error or dummy Orders are Existed";
        }
    }
}