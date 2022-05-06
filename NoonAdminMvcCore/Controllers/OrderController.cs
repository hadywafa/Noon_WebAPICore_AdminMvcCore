﻿using EFModel.Enums;
using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoonAdminMvcCore.Models;
using Repository.GenericRepository;
using Repository.UnitWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoonAdminMvcCore.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Admin)]
    public class OrderController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private readonly IGenericRepo<Order> _orderRepo;
        readonly IGenericRepo<User> _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IGenericRepo<OrderItem> _orderItems;

        public OrderController(IUnitOfWork unitOfWork, UserManager<User> userManager )
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _orderRepo = unitOfWork.Orders;
            _orderItems = unitOfWork.OrderItems;
        }
        //DeliveryStatus state
        public IActionResult Index(string? id, int? pageNumber, int? pageSize)
        {
            ViewData["PageSize"] = pageSize;

            int rowsPerPage = pageSize ?? 10;
            ViewBag.rowsPerPage = rowsPerPage;

            List<Order> orders;

            if (id != null)
                   orders = _orderRepo.FindAll(orders => orders.CustomerID == id).OrderByDescending(order => order.OrderDate).ToList();
            else
               orders = _orderRepo.GetAll().OrderByDescending(order => order.OrderDate).ToList();

            return View(EFModel.Models.PaginatedList<Order>.CreateAsync(orders, pageNumber ?? 1, rowsPerPage));
        }

        public IActionResult OrderProducts(int id)
        {
            
            var order = _orderRepo.Find(o => o.Id == id);
            var items=_orderItems.FindAll(item => item.OrderId == id,p=>p.Product);
            ViewBag.TotalPrice = order.TotalPrice;
            
            return View(items);
        }

        public IActionResult Edit(int id)
        {

            var order=_orderRepo.Find(o => o.Id == id);

            OrderViewModel model = new OrderViewModel {
                Id = order.Id,
                DeliveryStatus = order.DeliveryStatus,
                DeliveryStatusDescription = order.DeliveryStatusDescription,
                Shippers = _userManager.GetUsersInRoleAsync(AuthorizeRoles.Shipper).Result.ToList(),
                ShipperId = order.ShipperId,
            };
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(OrderViewModel model)
        {
            var order = _orderRepo.Find(o=>o.Id==model.Id);

            order.DeliveryStatus = model.DeliveryStatus;
            order.DeliveryStatusDescription = model.DeliveryStatusDescription;
            order.ShipperId = model.ShipperId;
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

    }
}
