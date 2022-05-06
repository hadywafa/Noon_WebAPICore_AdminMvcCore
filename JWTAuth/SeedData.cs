using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFModel.Enums;
using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Repository.GenericRepository;
using Repository.UnitWork;

namespace JWTAuth
{
    public class SeedData 
    {
        #region Inject Dependencies

        // Unit Of Work which is responsible on operations on Context
        readonly IUnitOfWork _unitOfWork;

        // User Repo which is responsible on operations on user
        readonly UserManager<User> _userManager;
        readonly RoleManager<IdentityRole> _roleManager;
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
        public SeedData(UserManager<User> userManager, IUnitOfWork unitOfWork,
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

        public async Task HwAddRoles()
        {
            bool isInserted = _roleManager.Roles.Any();
            if (!isInserted)
            {
                await _roleManager.CreateAsync(new IdentityRole(AuthorizeRoles.Admin));
                await _roleManager.CreateAsync(new IdentityRole(AuthorizeRoles.Customer));
                await _roleManager.CreateAsync(new IdentityRole(AuthorizeRoles.Seller));
                await _roleManager.CreateAsync(new IdentityRole(AuthorizeRoles.Shipper));
            }
        }

        public async Task HwAddUsers()
        {
            var team4 = new List<string>() { "Mo", "Kero", "Emad", "Hady" };
            var roles = new List<string>()
            {
                AuthorizeRoles.Admin, AuthorizeRoles.Customer, AuthorizeRoles.Seller, AuthorizeRoles.Shipper
            };
            bool isInserted = _userRepo.GetAll().Any();
            if (!isInserted)
            {
                foreach (string name in team4)
                {
                    foreach (var role in roles)
                    {
                        var user = new User
                        {
                            FirstName = name,
                            LastName = role,
                            Balance = 5000,
                            IsActive = true,
                            PhoneNumber = "0123456789",
                            Email = $"{name + role}@gmail.com",
                            UserName = $"{name + role}@gmail.com"
                        };

                        // Add user and save Context
                        await _userManager.CreateAsync(user, $"{name}@1234");

                        // Add To UserRoles table
                        await _userManager.AddToRoleAsync(user, role);
                        //_userRepo.Add(user);
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
            }
        }

        public void HwAddCategory()
        {
            bool isInserted = _catRepo.GetAll().Any();
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
                        Image = new Images() { Image = "12345" },
                    };
                    _catRepo.Add(cat);
                    _unitOfWork.Save();
                }
            }
        }

        public void HwAddProducts()
        {
            bool isInserted = _productRepo.GetAll().Any();
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
                        BuyingPrice = 50 * (i + 1),
                        SellingPrice = 100 * (i + 1),
                        Revenue = (100 * (i + 1)) - (50 * (i + 1)),
                        Quantity = 2 * (i + 1),
                        Discount = 0.2f,
                        IsActive = true,
                        AddedOn = DateTime.Now,
                        Weight = $"{5 * (i + 1)} kg",
                        Seller = _sellerRepo.Find(s => s.User.Email == "MoSeller@gmail.com"),
                        Images = new List<Images>()
                        {
                            new Images() { Image = $"Pro{i}Img{1}.jpg" },
                            new Images() { Image = $"Pro{i}Img{2}.jpg" },
                            new Images() { Image = $"Pro{i}Img{3}.jpg" },
                            new Images() { Image = $"Pro{i}Img{4}.jpg" },
                        }
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
                        BuyingPrice = 50 * (i + 1),
                        SellingPrice = 100 * (i + 1),
                        Revenue = (100 * (i + 1)) - (50 * (i + 1)),
                        Quantity = 2 * (i + 1),
                        Discount = 0.2f,
                        IsActive = true,
                        AddedOn = DateTime.Now,
                        Weight = $"{5 * (i + 1)} kg",
                        Seller = _sellerRepo.Find(s => s.User.Email == "EmadSeller@gmail.com"),
                        Images = new List<Images>()
                        {
                            new Images() { Image = $"Pro{i}Img{1}.jpg" },
                            new Images() { Image = $"Pro{i}Img{2}.jpg" },
                            new Images() { Image = $"Pro{i}Img{3}.jpg" },
                            new Images() { Image = $"Pro{i}Img{4}.jpg" },
                        }
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
                        BuyingPrice = 50 * (i + 1),
                        SellingPrice = 100 * (i + 1),
                        Revenue = (100 * (i + 1)) - (50 * (i + 1)),
                        Quantity = 2 * (i + 1),
                        Discount = 0.2f,
                        IsActive = true,
                        AddedOn = DateTime.Now,
                        Weight = $"{5 * (i + 1)} kg",
                        Seller = _sellerRepo.Find(s => s.User.Email == "KeroSeller@gmail.com"),
                        Images = new List<Images>()
                        {
                            new Images() { Image = $"Pro{i}Img{1}.jpg" },
                            new Images() { Image = $"Pro{i}Img{2}.jpg" },
                            new Images() { Image = $"Pro{i}Img{3}.jpg" },
                            new Images() { Image = $"Pro{i}Img{4}.jpg" },
                        }
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
                        BuyingPrice = 50 * (i + 1),
                        SellingPrice = 50 * (i + 1),
                        Revenue = (100 * (i + 1)) - (50 * (i + 1)),
                        Quantity = 2 * (i + 1),
                        Discount = 0.2f,
                        IsActive = true,
                        AddedOn = DateTime.Now,
                        Weight = $"{5 * (i + 1)} kg",
                        Seller = _sellerRepo.Find(s => s.User.Email == "HadySeller@gmail.com"),
                        Images = new List<Images>()
                        {
                            new Images() { Image = $"Pro{i}Img{1}.jpg" },
                            new Images() { Image = $"Pro{i}Img{2}.jpg" },
                            new Images() { Image = $"Pro{i}Img{3}.jpg" },
                            new Images() { Image = $"Pro{i}Img{4}.jpg" },
                        }
                    };
                    _productRepo.Add(pro);
                    _unitOfWork.Save();
                }
            }
        }

        public async Task HwAddOrder()
        {
            bool isInserted = _orderRepo.GetAll().Any();
            if (!isInserted)
            {
                #region Mo Orders

                var orderMo1 = new Order()
                {
                    User = await _userManager.FindByEmailAsync("MoCustomer@gmail.com"),
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
                    User = await _userManager.FindByEmailAsync("MoCustomer@gmail.com"),
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
                    User = await _userManager.FindByEmailAsync("KeroCustomer@gmail.com"),
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
                    User = await _userManager.FindByEmailAsync("KeroCustomer@gmail.com"),
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
                    User = await _userManager.FindByEmailAsync("EmadCustomer@gmail.com"),
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
                    User = await _userManager.FindByEmailAsync("EmadCustomer@gmail.com"),
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
                    User = await _userManager.FindByEmailAsync("HadyCustomer@gmail.com"),
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
                    User = await _userManager.FindByEmailAsync("HadyCustomer@gmail.com"),
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
            }
        }

    }
}