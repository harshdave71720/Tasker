using System.ComponentModel.DataAnnotations;

namespace Tasker.WebAPI.Models
{
    public class RegisterUserModel
    {
        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
