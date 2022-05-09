using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EFModel.Enums;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Repository.GenericRepository;
using Repository.UnitWork;
using BL.Helpers;
using BL.ViewModels.ResponseVModels;

namespace JWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        #region Inject Product Repository in Author Controller
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        readonly IGenericRepo<Customer> _customerRepo;
        private readonly IGenericRepo<Product> _productRepo;
        private readonly IGenericRepo<CustProCart> _custProCartRepo;

        public CartController(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productRepo = _unitOfWork.Products;
            _customerRepo = _unitOfWork.Customers;
            _custProCartRepo = _unitOfWork.CustProCarts;
            _mapper = mapper;
            //
        }

        #endregion

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            // get user from request
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            //it will return List of CustProCart object
            // custId  productId  quantity 
            //   1          1       5
            //   1          2       3
            //   1          3       7
            var carts = await _custProCartRepo.GetAll().Include(x=>x.Product).ToListAsync();
            if (carts == null)
            {
                return Ok("Your Cart is Empty");
            }
            foreach (var item in carts)
            {
                item.Product.ImageThumb.ToImageUrl();
            }
            var vmCartProducts = _mapper.Map<List<CustProCart>,List<VmCartProduct>>(carts);

            return Ok(vmCartProducts);
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpPost("Add")]
        public async Task<IActionResult> AddToCart([FromQuery]int proId  ,[FromQuery] int count)
        {
            var pro = await _productRepo.GetById(proId);
            if (pro== null || pro.IsAvailable == false)
            {
                return  BadRequest("There is no Product with that id");
            }
            // get user from request
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            //var customer = await _customerRepo.GetById(userId );
            var customer = await _customerRepo.Find(x => x.Id == userId  );
            //check if item is exist
            var cartItem = await _custProCartRepo.Find(x => x.Customer.Id == userId && x.Product.Id == proId);
            if (cartItem == null )
            {
                await _custProCartRepo.Add(new CustProCart() {Customer = customer , Product = pro , Quantity = count});
                await _unitOfWork.Save();
                return Ok("item added successfully to your Cart");
            }
            //update only quantity
            if (cartItem.Quantity + count > pro.Quantity )
            {
                cartItem.Quantity = pro.Quantity;
                await _unitOfWork.Save();
                return Ok("item quantity updated successfully");
            }
            cartItem.Quantity += count;
            await _unitOfWork.Save();
            // update cart
            return Ok("item added successfully to your Cart");
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpPut("Update")]
        public  async Task<IActionResult> UpdateQuantity([FromQuery]int proId  ,[FromQuery] int count)
        {
            var pro = await _productRepo.GetById(proId);
            if (pro== null || pro.IsAvailable == false)
            {
                return BadRequest("There is no Product with that id");
            }
            // get user from request
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            //check if item is exist
            var cartItem = await _custProCartRepo.Find(x => x.Customer.User.Id == userId && x.Product.Id == proId);
            if (cartItem == null )
            {
                return Ok("Item is Not Existed in your Cart");
            }
            //update only quantity
            if (cartItem.Quantity + count > pro.Quantity )
            {
                cartItem.Quantity = pro.Quantity;
                await _unitOfWork.Save();
                return Ok("item quantity updated successfully");
            }
            cartItem.Quantity += count;
            await _unitOfWork.Save();
            // update cart
            return Ok("item quantity updated successfully");
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpDelete("Remove")]
        public  async Task<IActionResult> RemoveFromCart([FromQuery]int proId)
        {
            var pro = await _productRepo.GetById(proId);
            if (pro== null || pro.IsAvailable == false)
            {
                return BadRequest("There is no Product with that id");
            }
            // get user from request
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            var cartItem = await _custProCartRepo.Find(x => x.Customer.User.Id == userId && x.Product.Id == proId);
            if (cartItem == null )
            {
                return Ok("Item is Not Existed in your Cart");
            }
            await _custProCartRepo.Remove(cartItem);
            await _unitOfWork.Save();
            return Ok("item Removed successfully from your Cart");
        }
    }
}
