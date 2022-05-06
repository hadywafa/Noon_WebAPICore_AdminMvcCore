using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BL.Helper;
using EFModel.Enums;
using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Repository.GenericRepository;
using Repository.UnitWork;

namespace JWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        #region Inject Product Repository in Author Controller
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        readonly IGenericRepo<Customer> _customerRepo;
        private readonly IGenericRepo<Product> _productRepo;
        private readonly IGenericRepo<Cart> _cartRepo;
        private readonly IGenericRepo<CartProducts> _cartProducts;

        public CartController(IUnitOfWork unitOfWork , UserManager<User> userManager , RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _cartRepo = _unitOfWork.Carts;
            _productRepo = _unitOfWork.Products;
            _customerRepo = _unitOfWork.Customers;
            _cartProducts = _unitOfWork.CartProducts;
            //
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #endregion

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpGet("test")]
        public  string Test()
        {
            // Get User from Claims
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user Name
            var useEmail = User.FindFirstValue(ClaimTypes.Email); // will give the user's Email
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            return $"{userName}  \n   {useEmail}  \n {userId}";
        }


        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpGet("GetAll")]
        public  List<CartProducts> GetAll()
        {
            // get user from request
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            // get Customer include its cart
            var cartItems = _cartProducts.FindAll(x => x.Cart.Customer.Id == userId, y => y.Products, z => z.Cart).ToList();
            //sending products
            //var cartProducts = _cartProducts.GetAll().Where(x =>  x.Cart.Id == cart.Id);
            return cartItems;
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpPost("Add")]
        public  async Task<IActionResult> AddToCart([FromQuery]int proId  ,[FromBody] int count)
        {
            var pro = _productRepo.GetById(proId);
            if (pro== null)
            {
                return BadRequest("There is no Product with that id");
            }
            // get user from request
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            // get Customer include its cart
            var cart = _cartRepo.Find(x => x.Customer.Id == userId);
            if (cart is null)
            {
                 cart = new Cart();
                 cart.Customer = _customerRepo.GetById(userId);
            }

            var cartProducts = _cartProducts.GetAll().FirstOrDefault(x => x.Products.Id == pro.Id && x.Cart.Id == cart.Id);
            if (cartProducts is null)
            {
                _cartProducts.Add(new CartProducts() {Cart = cart , Products = pro  , Quantity = count});
                _unitOfWork.Save();
                return Ok("item added successfully");

            }

            if (cartProducts.Quantity + count > cartProducts.Products.Quantity)
            {
                cartProducts.Quantity = cartProducts.Products.Quantity;
                _unitOfWork.Save();
                return Ok("item added successfully");
            }
            cartProducts.Quantity += count;
            _unitOfWork.Save();
            // update cart
            return Ok("item added successfully");
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpPost("Update")]
        public  async Task<IActionResult> UpdateQuantity([FromQuery]int proId  ,[FromBody] int count)
        {
            var pro = _productRepo.GetById(proId);
            if (pro== null)
            {
                return BadRequest("There is no Product with that id");
            }
            // get user from request
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            // get Customer include its cart
            var cart = _cartRepo.Find(x => x.Customer.Id == userId);

            var cartProducts = _cartProducts.GetAll().FirstOrDefault(x => x.Products.Id == pro.Id && x.Cart.Id == cart.Id);
            if (cartProducts is null)
            {
                return Ok("There is no Product with that id in your cart");
            }

            if ( count > cartProducts.Products.Quantity)
            {
                cartProducts.Quantity = cartProducts.Products.Quantity;
                _unitOfWork.Save();
                return Ok("item Updated successfully");
            }
            cartProducts.Quantity = count;
            _unitOfWork.Save();
            // update cart
            return Ok("item Updated successfully");
        }


        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpDelete("Remove")]
        public  async Task<IActionResult> RemoveFromCart([FromQuery]int proId)
        {
            var pro = _productRepo.GetById(proId);
            if (pro== null)
            {
                return BadRequest("There is no Product with that id");
            }
            // get user from request
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            // get Customer include its cart
            var cart = _cartRepo.Find(x => x.Customer.Id == userId);

            var cartProducts = _cartProducts.GetAll().FirstOrDefault(x => x.Products.Id == pro.Id && x.Cart.Id == cart.Id);
            if (cartProducts is null)
            {
                return Ok("There is no Product with that id in your cart");
            }
            _cartProducts.Remove(cartProducts);
            _unitOfWork.Save();
            // update cart
            return Ok("Item Removed Succeeded");
        }

    }
}
