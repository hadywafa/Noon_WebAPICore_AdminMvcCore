using BL.AppPolicy;
using EFModel.Enums;
using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoonAdminMvcCore.Models;
using Repository.GenericRepository;
using Repository.UnitWork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace NoonAdminMvcCore.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Admin)]
    public class HomeController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private readonly IGenericRepo<Order> _orderRepo;
        readonly IGenericRepo<User> _userRepository;
        readonly IGenericRepo<Customer> _customerRepo;
        readonly IGenericRepo<Seller> _sellerRepo;
        readonly IGenericRepo<Product> _productRepository;
        readonly IGenericRepo<Category> _categoryRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;

            _categoryRepository = _unitOfWork.Categories;
            _productRepository = _unitOfWork.Products;

            _orderRepo = _unitOfWork.Orders;

            _userRepository = _unitOfWork.Users;
            _customerRepo = _unitOfWork.Customers;
            _sellerRepo = _unitOfWork.Sellers;

        }


        public IActionResult Index()
        {
            // Orders Section >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            var todayOrders = _orderRepo.FindAll(
                o => o.OrderDate.Year == DateTime.Now.Year
                && o.OrderDate.Month == DateTime.Now.Month
                && o.OrderDate.Day == DateTime.Now.Day);
            int todayOrdersCount = todayOrders.Count();

            String allOrdersCount = _orderRepo.GetAll().Count().ToString() ?? "0";

            var recentOrders = _orderRepo.GetAll(o => o.User).OrderByDescending(o => o.OrderDate).Take(5);

            // Revenue Section
            decimal todayRevenue = default;
            foreach (var order in todayOrders)
            {
                todayRevenue += order.TotalRevenue;
            }

            // Customers Section >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            var todayCustomers = _customerRepo.FindAll(
                c => c.createdAt.Year == DateTime.Now.Year
                && c.createdAt.Month == DateTime.Now.Month
                && c.createdAt.Day == DateTime.Now.Day);

            string todayCustomersCount = todayCustomers.Count().ToString() ?? "0";
            string allCustomerCount = _customerRepo.GetAll().Count().ToString() ?? "0";

            // Product Section
            string allProductsCount = _productRepository.GetAll().Count().ToString() ?? "0";

            // Category Section
            string allCategoiesCount = _categoryRepository.GetAll().Count().ToString() ?? "0";

            // Seller Section >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            var todaySellers = _sellerRepo.FindAll(
                s => s.createdAt.Year == DateTime.Now.Year
                && s.createdAt.Month == DateTime.Now.Month
                && s.createdAt.Day == DateTime.Now.Day);

            string todaySellersCount = todaySellers.Count().ToString() ?? "0";

            string allSellerCount = _sellerRepo.GetAll().Count().ToString() ?? "0";

            // Send data by ViewBag >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            ViewBag.todayOrdersCount = todayOrdersCount;
            ViewBag.allOrdersCount = allOrdersCount;
            ViewBag.recentOrders = recentOrders;
            ViewBag.todayRevenue = todayRevenue;
            ViewBag.todayCustomersCount = todayCustomersCount;
            ViewBag.allCustomerCount = allCustomerCount;
            ViewBag.allProductsCount = allProductsCount;
            ViewBag.allCategoiesCount = allCategoiesCount;
            ViewBag.todaySellersCount = todaySellersCount;
            ViewBag.allSellerCount = allSellerCount;
            ViewBag.DeliveryStatus = new List<DeliveryStatus>();

            return View(recentOrders);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
