using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BL.AppPolicy;
using BL.Helper;
using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
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

        public CartController(IUnitOfWork unitOfWork , UserManager<User> userManager , RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _cartRepo = _unitOfWork.Carts;
            _productRepo = _unitOfWork.Products;
            _customerRepo = _unitOfWork.Customers;
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

        public async Task<List<Product>> GetAll()
        {
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            var customer = _customerRepo.GetById(userId);
            // get user from request
            // get user cart
            // get products from cart
            return new List<Product>();
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpGet("Add")]
        public  async Task<IActionResult> AddToCart(int proId)
        {
            // get user from request
            // get user cart
            // add product from cart by product id
            // update cart
            return Ok();
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpGet("Remove")]
        public  async Task<IActionResult> RemoveFromCart(int proId)
        {
            // get user from request
            // get user cart
            // remove product from cart by product id
            // update cart
            return Ok();
        }

    }
}
