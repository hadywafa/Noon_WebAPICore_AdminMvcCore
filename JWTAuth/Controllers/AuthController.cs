using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BL.ViewModels.RequestVModels;
using EFModel.Models;
using Microsoft.AspNetCore.Identity;
using Repository.CustomRepository.AuthRepo;
using Repository.GenericRepository;
using Repository.UnitWork;

namespace JWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Inject AuthRepo
        private readonly UserManager<User> _userManager;
        private readonly IGenericRepo<User> _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthRepo _authRepo;

        public AuthController(IUnitOfWork unitOfWork , UserManager<User> userManager)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _authRepo = _unitOfWork.GetAuthRepo();
            _userRepo = _unitOfWork.Users;
        }

        #endregion

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] VmRegisterUser vmRegisterUser)
        {
            //check successful data binding from HTTP Request to VmRegister object
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authRepo.RegisterAsync(vmRegisterUser);

            //check successful registration process  if fails => return all error information
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInAsync([FromBody] VmSignInUser vmSignInUser)
        {
            //check successful data binding from HTTP Request to VmRegister object
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authRepo.SignInAsync(vmSignInUser);
            //check successful registration process  if fails => return all error information
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            var user = await _userRepo.Find(x => x.Email == vmSignInUser.Email);
            result.FirstName = user.FirstName;
            result.LastName = user.LastName;
            return Ok(result);
        }
    }
}
