using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repository.GenericRepository;
using Repository.UnitWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoonAdminMvcCore.Controllers
{
    public class OrderController : Controller
    {

        private IUnitOfWork _unitOfWork;
        private readonly IGenericRepo<Order> _orderRepo;
        readonly IGenericRepo<User> _userRepository;
        private readonly UserManager<User> _userManager;



        public OrderController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _orderRepo = unitOfWork.Orders;
        }

        public IActionResult Index()
        {

            IQueryable<Order> orders;
            orders=_orderRepo.GetAll();

            return View(orders);
        }
    }
}
