namespace Tasker.WebAPI.Models
{
    public class AuthenticatedUserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Token { get; set; }
    }
}
