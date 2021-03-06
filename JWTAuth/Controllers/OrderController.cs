using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using EFModel.Enums;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Repository.GenericRepository;
using Repository.UnitWork;
using Microsoft.AspNetCore.Identity;
using EFModel.Models;
using System;
using System.Collections.ObjectModel;
using JWTAuth.ViewModels;
using System.Collections.Generic;
using AutoMapper;
using BL.Helpers;
using BL.ViewModels.ResponseVModels;

namespace JWTAuth.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        #region Inject Product Repository in Author Controller
        readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepo<Customer> _customerRepo;
        private readonly IGenericRepo<Product> _productRepo;
        private readonly IGenericRepo<Shipper> _shipperRepo;
        private readonly IGenericRepo<Order> _orderRepo;
        private readonly IGenericRepo<CustProCart> _custProCartRepo;
        private readonly IGenericRepo<OrderItem> _orderItemsRepo;
        private readonly IGenericRepo<User> _userRepo;

        public OrderController(IUnitOfWork unitOfWork, UserManager<User> userManager , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _productRepo = _unitOfWork.Products;
            _customerRepo = _unitOfWork.Customers;
            _orderRepo = _unitOfWork.Orders;
            _shipperRepo = _unitOfWork.Shippers;
            _custProCartRepo=_unitOfWork.CustProCarts;
            _orderItemsRepo = _unitOfWork.OrderItems;
            _userManager = userManager;
            _mapper = mapper;
            _userRepo = _unitOfWork.Users;
        }

        #endregion

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;

            var orders = _orderRepo.GetAll().Where(c=>c.Customer.Id == userId);

            return Ok(orders);
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpPost("Add")]
        public async Task<IActionResult> PlaceOrder([FromQuery] PaymentMethod PaymentMethod, [FromQuery] string addressId)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;
            var carts = await _custProCartRepo.GetAll().Include(x => x.Product).Where(c => c.Customer.Id == userId).ToListAsync();
            if(carts==null)
            {
                return BadRequest();
            }
            var _orderitems = new Collection<OrderItem>();

            foreach (var item in carts)
            {
                var oi = new OrderItem
                {
                    Quantity = item.Quantity,
                    Product = item.Product
                };

                var product = await _productRepo.Find(p => p.Id == item.Product.Id);

                if (product.Quantity < oi.Quantity)
                    return BadRequest();

                product.Quantity -= oi.Quantity;

                _orderitems.Add(oi);
            }

            var order = new Order()
            {
                User = await _userManager.FindByIdAsync(userId),
                Customer = await _customerRepo.Find(c => c.User.Id == userId),
                Shipper = await _shipperRepo.Find(s => s.User.Email == "MoShipper@gmail.com"),
                Discount = (decimal)0.0,
                PaymentMethod = PaymentMethod,
                OrderItems = _orderitems,
                AddressId = int.Parse(addressId)
            };
           
            order.CalcTotalPrice();

            await _orderRepo.Add(order);

            //reduce Customer Balance
            if (order.PaymentMethod == PaymentMethod.NoonBalance)
            {
                var user = await _userRepo.Find(c => c.Id == userId);

                if (user.Balance < order.TotalPrice)
                    return NotFound();

                user.Balance -= order.TotalPrice;
            }

            foreach (var item in carts)
            {
                var cartItem = await _custProCartRepo.Find(x => x.Customer.User.Id == userId && x.Product.Id == item.Product.Id);
                await _custProCartRepo.Remove(cartItem);
            }

            await _unitOfWork.Save();

            return Ok(order);
        }



        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpGet("OrderDetails")]
        public async Task<IActionResult> OrderDetails([FromQuery]int id)
        {

            Order order = await _orderRepo.Find(o => o.Id == id);
            if (order == null)
                return BadRequest();

            var items = _orderItemsRepo.FindAll(item => item.OrderId == id, p => p.Product);

            Collection<Product> productList=new Collection<Product>() ;
            foreach(var product in items)
            {
                product.Product.ImageThumb.ToImageUrl();
                productList.Add(product.Product);
            }

            var orderDetails = new OrderDetails {

                id = order.Id,
                deliveryStatus = order.DeliveryStatus,
                deliveryStatusDescription = order.DeliveryStatusDescription,
                totalPrice = order.TotalPrice,
                products = productList
            };


            return Ok(orderDetails);
        }

        [HttpGet("GetOrderDetails")]
        public async Task<IActionResult> GetOrderDetails([FromQuery]int id)
        {

            var order = await _orderRepo.Find(o => o.Id == id , x=>x.OrderItems);
            if (order == null)
                return BadRequest();

            else
            {
                var vmOrder = _mapper.Map<Order, VmOrder >(order);
                return Ok(order);
            }
        }

    }
}
