using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EFModel.Enums;
using Microsoft.AspNetCore.Authorization;

namespace JWTAuth.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [Authorize(Roles = AuthorizeRoles.Customer)]
        [Route("CreateOrder")]
        [HttpGet]
        public async Task<ActionResult> CreateOrder()
        {

            return Accepted("Order Added Successfully");
        }

        [Authorize( Roles = AuthorizeRoles.Admin + "," + AuthorizeRoles.Seller)]
        [Route("AddProduct")]
        [HttpGet]
        public async Task<ActionResult> AddProduct()
        {
            return Accepted("Product Added Successfully");
        }

        [Authorize(Roles = AuthorizeRoles.Admin + "," + AuthorizeRoles.Seller + "," + AuthorizeRoles.Shipper)]
        [Route("AddAddress")]
        [HttpGet]
        public async Task<ActionResult> AddAddress()
        {
            return Accepted("Address Added Successfully");
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [Route("AddToCart")]
        [HttpGet]
        public async Task<ActionResult> AddToCart()
        {
            return Accepted("Product Added Successfully");
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [Route("RemoveFromCart")]
        [HttpGet]
        public async Task<ActionResult> RemoveFromCart()
        {
            return Accepted("Product Removed Successfully");
        }

        [Authorize(Roles = AuthorizeRoles.Shipper)]
        [Route("GetOrderbyShipperId")]
        [HttpGet]
        public async Task<ActionResult> GetOrderbyShipperId()
        {
            return Accepted("Here is ");
        }
    }
}