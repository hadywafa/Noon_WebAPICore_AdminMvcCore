using System.ComponentModel.DataAnnotations;

namespace BL.ViewModels.RequestVModels
{
    public class VmSignInUser
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
