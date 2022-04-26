using System.ComponentModel.DataAnnotations;
using EFModel.Models;

namespace BL.ViewModels.RequestVModels
{
    public class VmRegisterUser
    {
        [Required,StringLength(100)]
        public string FirstName { get; set; }

        [Required,StringLength(100)]
        public string LastName { get; set; }

        [Required,StringLength(50)]
        public string UserName { get; set; }

        [Required,StringLength(128)]
        public string Email { get; set; }

        [Required,StringLength(250)]
        public string Password { get; set; }

        [Required,StringLength(50)]
        public string Role { get; set; }
    }

    public static class VmRegisterUserExtensionMethods
    {
        public static User ToUser(this VmRegisterUser signUpUser)
        {
            return new User()
            {
                FirstName = signUpUser.FirstName,
                LastName = signUpUser.LastName,
                Email = signUpUser.Email,
                PasswordHash = signUpUser.Password,
                UserName = signUpUser.Email,
            };
        }
    }
}