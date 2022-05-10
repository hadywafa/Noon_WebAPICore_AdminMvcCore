using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EFModel.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using BL.ViewModels.RequestVModels;
using EFModel.Models;
using Repository.GenericRepository;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Identity;
using Repository.UnitWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace JWTAuth.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Injection

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IGenericRepo<User> _userRepo;
        private readonly IGenericRepo<Customer> _customerRepo;
        private readonly IGenericRepo<Address> _addressRepo;


        public UserController(IUnitOfWork unitOfWork , UserManager<User> userManager)
        {
            _userManager = userManager;
            _userRepo = unitOfWork.Users;
            _customerRepo = unitOfWork.Customers;
            _unitOfWork = unitOfWork;
            _addressRepo = unitOfWork.Addresses;
        }
        

        #endregion

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [Route("Addresses")]
        [HttpGet]
        public async Task<ActionResult> GetAllAddresses()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;
            var addresses = await this._addressRepo.GetAll().Where(x=>x.User.Id == userId)
                .Select(a => new
                    { City = a.City, Street = a.Street, PostalCode = a.PostalCode, Id = a.Id, IsPrimary = a.IsPrimary , addressPhone = a.AddressPhone})
                .ToListAsync();

            return Ok(addresses);
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpPost("AddAddress")]
        public async Task<IActionResult> AddAddress([FromBody]VmAddress _address)
        {

            var userId = User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;
            var user = await _userRepo.Find(x => x.Id == userId  );

            await _addressRepo.Add(new Address()
            {
                User = user,Street = _address.Street , City = _address.City , PostalCode = _address.PostalCode, IsPrimary = false , AddressPhone = _address.AddressPhone
            });
            await _unitOfWork.Save();
            return StatusCode(200);
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpPut("UpdateAddress")]
        public async Task<IActionResult> UpdateAddress([FromBody]VmAddress newAddress)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;
            var address = await _addressRepo.Find(x => x.User.Id == userId && x.Id == newAddress.Id);

            address.City = newAddress.City;
            address.PostalCode = newAddress.PostalCode;
            address.Street = newAddress.Street;
            if (newAddress.IsPrimary )
            {
                var userAddresses = await _addressRepo.FindAll(x => x.User.Id == userId).ToListAsync();

                foreach (Address a in userAddresses)
                {
                    a.IsPrimary = false;
                    await _addressRepo.Update(a);
                    await _unitOfWork.Save();
                }
            }
            address.IsPrimary = newAddress.IsPrimary;
            await _addressRepo.Update(address);
            await _unitOfWork.Save();
            return StatusCode(200,new {MessagePack="Address is Updated Successfully"});
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpDelete("DeleteAddress")]
        public async Task<IActionResult> DeleteAddress([FromQuery]int addressId)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;
            var address = await _addressRepo.Find(x => x.User.Id == userId && x.Id == addressId);

            if (address == null)
            {
                return StatusCode(200,new {MessagePack="Address is not exist"});
            }
            await _addressRepo.Remove(address);
            await _unitOfWork.Save();
            return StatusCode(200,new {MessagePack="Address is removed Successfully"});
        }

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpPut("UpdateUserName")]
        public async Task<IActionResult> UpdateUserName([FromQuery]string first , [FromQuery] string last)
        {
          
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            //var customer = await _customerRepo.GetById(userId );
            var user = await _userRepo.Find(x => x.Id == userId );
            user.FirstName = first;
            user.LastName = last;
            await _userRepo.Update(user);
            await _unitOfWork.Save();
            return StatusCode(200 , new {message="Your name updated Successfully"});
        }
        
        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody]VmChangePassword passObj)
        {
          
            var userId= User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value; //Get  Custom Claim User Id
            var user = await _userRepo.Find(x => x.Id == userId );
            var result = await _userManager.ChangePasswordAsync(user, passObj.OldPassword, passObj.NewPassword);
            if (!result.Succeeded)
            {
                return Ok(result.Errors);
            }
            return StatusCode(200 , new {message="Password updated Successfully"});

        }
    }
}