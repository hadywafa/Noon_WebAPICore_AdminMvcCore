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

namespace JWTAuth.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        #region Inject Product Repository in Author Controller
        readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepo<Customer> _customerRepo;
        private readonly IGenericRepo<Product> _productRepo;
        private readonly IGenericRepo<Shipper> _shipperRepo;
        private readonly IGenericRepo<Order> _orderRepo;
        private readonly IGenericRepo<CustProCart> _custProCartRepo;
        private readonly IGenericRepo<User> _userRepo;

        public OrderController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _productRepo = _unitOfWork.Products;
            _customerRepo = _unitOfWork.Customers;
            _orderRepo = _unitOfWork.Orders;
            _shipperRepo = _unitOfWork.Shippers;
            _custProCartRepo=_unitOfWork.CustProCarts;
            _userManager = userManager;
            _userRepo = _unitOfWork.Users;
        }

        #endregion

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;

            var orders = _orderRepo.GetAll().Where(c=>c.Customer.Id == userId);

            var OrdersVM = new Collection<OrderVM>();

            foreach(var item in orders)
            {
                var orderVM = new OrderVM
                {
                    OrderId=item.Id,
                    DeliveryStatus=item.DeliveryStatus,
                    TotalPrice=item.TotalPrice
                };

                OrdersVM.Add(orderVM);
            }

            return Ok(OrdersVM);
        }




        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpPost("Add")]
        public async Task<IActionResult> PlaceOrder([FromQuery] PaymentMethod PaymentMethod, [FromQuery] string addressId)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;
            var carts = await _custProCartRepo.GetAll().Include(x => x.Product).Where(c => c.Customer.Id == userId).ToListAsync();
            var _orderitems = new Collection<OrderItem>();

            foreach (var item in carts)
            {
                var oi = new OrderItem
                {
                    Quantity = item.Quantity,
                    Product = item.Product
                };

                _orderitems.Add(oi);
            }

            var order = new Order()
            {
                User = await _userManager.FindByIdAsync(userId),
                Customer = await _customerRepo.Find(c => c.User.Id == userId),
                Shipper = await _shipperRepo.Find(s => s.User.Email == "MoShipper@gmail.com"),
                Discount = (decimal)0.0,
                DeliveryStatus = DeliveryStatus.Processing,
                PaymentMethod = PaymentMethod,
                OrderDate = DateTime.Now,
                OrderItems = _orderitems,
                AddressId = int.Parse(addressId)
            };

            order.CalcTotalPrice();
            await _orderRepo.Add(order);

            //reduce product quantity
            //foreach (var orderItem in order.OrderItems)
            //{
            //    var product = orderItem.Product;
            //    var reducedQuantity = orderItem.Quantity;
            //    product.Quantity -= reducedQuantity;
            //}



            //reduce Customer Balance
            //if (order.PaymentMethod == PaymentMethod.NoonBalance)
            //{
            //    var user = await _userRepo.Find(c => c.Id == userId);

            //    if (user.Balance < order.TotalPrice)
            //        return NotFound();

            //    user.Balance -= order.TotalPrice;
            //}

            //await _unitOfWork.Save();


            foreach (var item in carts)
            {
                var cartItem = await _custProCartRepo.Find(x => x.Customer.User.Id == userId && x.Product.Id == item.Product.Id);
                await _custProCartRepo.Remove(cartItem);
            }
            await _unitOfWork.Save();

            return Ok();
        }









    }
}
