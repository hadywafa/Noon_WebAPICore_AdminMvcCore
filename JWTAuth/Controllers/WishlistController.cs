using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BL.ViewModels.ResponseVModels;
using EFModel.Enums;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Repository.GenericRepository;
using Repository.UnitWork;

namespace JWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        #region Inject Product Repository in Author Controller
        
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepo<Customer> _customerRepo;
        private readonly IGenericRepo<Product> _productRepo;
        private readonly IGenericRepo<CustProWishlist> _custProWishlistRepo;

        public WishlistController(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _customerRepo = _unitOfWork.Customers;
            _productRepo = _unitOfWork.Products;
            _custProWishlistRepo = _unitOfWork.CustProWishlists;
        }

        #endregion

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            // get user from request
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            //it will return List of CustProWishlist object
            // custId  productId  
            //   1          1     
            //   1          2     
            //   1          3     
            var wishlists = await _custProWishlistRepo.GetAll().Include(x=>x.Customer).Include(x=>x.Product).ToListAsync();
            if (wishlists == null)
            {
                return Ok("Your Cart is Empty");
            }

            var vmProducts = _mapper.Map<List<CustProWishlist>, List<VmWishlistProduct>>(wishlists);
            //need to map to IWishlistItem[] in Angular
            return Ok(vmProducts);
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpPost("Add")]
        public async Task<IActionResult> AddToWishlist([FromQuery]int proId )
        {
            var pro = await _productRepo.GetById(proId);
            if (pro== null)
            {
                return  BadRequest("There is no Product with that id");
            }
            // get user from request
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            var customer = await _customerRepo.Find(x => x.Id == userId);
            // get Customer include its cart
            var wishlistItem = await _custProWishlistRepo.Find(x => x.Customer.Id == userId && x.Product.Id == proId);
            if (wishlistItem is not null)
            {
                return Ok("item is already in your wishlist");
            }

            await _custProWishlistRepo.Add(new CustProWishlist() {Customer = customer , Product = pro });
            await _unitOfWork.Save();
            return Ok("item added successfully to your Wishlist");
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpDelete("Remove")]
        public  async Task<IActionResult> RemoveFromWishlist([FromQuery]int proId)
        {
            var pro = await _productRepo.GetById(proId);
            if (pro== null)
            {
                return BadRequest("There is no Product with that id");
            }
            // get user from request
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            var wishlistItem = await _custProWishlistRepo.Find(x => x.Customer.Id == userId && x.Product.Id == proId);
            if (wishlistItem == null )
            {
                return Ok("Item is Not Existed in your wishlist");
            }
            await _custProWishlistRepo.Remove(wishlistItem);
            await _unitOfWork.Save();
            return Ok("item Removed successfully from your wishlist");
        }

    }
}
