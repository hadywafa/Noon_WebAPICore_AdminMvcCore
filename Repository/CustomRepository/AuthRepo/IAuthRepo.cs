using System.Threading.Tasks;
using BL.ViewModels.RequestVModels;
using BL.ViewModels.ResponseVModels;

namespace Repository.CustomRepository.AuthRepo
{
    public interface IAuthRepo
    {
       Task<VmAuthUser> RegisterAsync(VmRegisterUser model);
       //Step:-1
       //1-check that there is no email looks like that new mail
       //2-check there is no userName looks like that new userName
       //Step:-2
       //1-create user
       //2-if there error => return vm authUser with empty errors information and token
       //3-else => welcome for successfully registration , now convert vmRegister to user
       //4-add him to roles
       //5-create token
       //6-return vm authUser with that token and main information
       Task<VmAuthUser> SignInAsync(VmSignInUser model);
       //Step:-1
       //1-check that email is exist
       //2-check that password is correct
       //Step:-2
       //1-create token
       //2-get main information about user such as its roles
       //3-return vm authUser with that token and main information
 
    }
}
