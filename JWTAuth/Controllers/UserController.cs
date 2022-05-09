using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EFModel.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using EFModel.Models;
using Repository.GenericRepository;
using EFModel.Models.EFModels;
using Repository.UnitWork;

namespace JWTAuth.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {


        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepo<Address> _address;



        public UserController(IUnitOfWork unitOfWork)
        {

            this._unitOfWork = unitOfWork;
            this._address = unitOfWork.Addresses;

        }
      
     

 

        //[Authorize(Roles = AuthorizeRoles.Customer)]
        [Route("Addresess")]
        [HttpGet]
        public async Task<ActionResult> GetAdresses()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;

            var addresses = this._address.GetAll().Where(u => u.User.Id == userId);

         

            return Ok(addresses);
        }



        [HttpPost]

        public async Task<ActionResult> AddAddress(Address address)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;

            var addresses = this._address.Add(address);



            return Ok(addresses);

        }



    }
}