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
        readonly IGenericRepo<Customer> _customerRepo;
        private readonly IGenericRepo<Product> _productRepo;
        private readonly IGenericRepo<Shipper> _shipperRepo;
        private readonly IGenericRepo<Order> _orderRepo;
        private readonly IGenericRepo<CustProCart> _custProCartRepo;



        public OrderController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _productRepo = _unitOfWork.Products;
            _customerRepo = _unitOfWork.Customers;
            _orderRepo = _unitOfWork.Orders;
            _shipperRepo = _unitOfWork.Shippers;
            _custProCartRepo=_unitOfWork.CustProCarts;
            _userManager = userManager;
            //
        }

        #endregion

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok();
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpPost("Add")]
        public async Task<IActionResult> PlaceOrder([FromQuery] bool payment, [FromQuery] string addressId)
        {


            var userId = User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;

            var carts = await _custProCartRepo.GetAll().Include(x => x.Product).ToListAsync();

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
                IsPaid = payment,
                DeliveryStatus = DeliveryStatus.Processing,
                PaymentMethod =payment? PaymentMethod.Ondelivered:PaymentMethod.Chash,
                OrderDate = DateTime.Now,
                OrderItems = _orderitems
                   
            };
           
            
            
            
      
            
            order.CalcTotalPrice();
            await _orderRepo.Add(order);
            //reduce product quantity
            foreach (var orderItem in order.OrderItems)
            {
                var product = orderItem.Product;
                var reducedQuantity = orderItem.Quantity;
                product.Quantity -= reducedQuantity;
            }

            //reduce Customer Balance 
            //if (order.PaymentMethod == PaymentMethod.Chash)
            //{
            //    var user = await _userRepo.Find(c => c.Id == orderMo1.Customer.Id);
            //    user.Balance -= order.TotalPrice;
            //}

            await _unitOfWork.Save();

         





            return Ok("");
        }

    }
}
