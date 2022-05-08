using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
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
    public class OrderController : ControllerBase
    {
        #region Inject Product Repository in Author Controller
        
        private readonly IUnitOfWork _unitOfWork;
        readonly IGenericRepo<Customer> _customerRepo;
        private readonly IGenericRepo<Product> _productRepo;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _productRepo = _unitOfWork.Products;
            _customerRepo = _unitOfWork.Customers;
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
        public async Task<IActionResult> PlaceOrder([FromQuery]int proId  ,[FromQuery] int count)
        {
            return Ok("");
        }

    }
}
