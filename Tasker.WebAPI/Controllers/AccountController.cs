using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tasker.Core.Helpers;
using Tasker.Identity.Application.Services;
using Tasker.WebAPI.Models;

namespace Tasker.WebAPI.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserIdentityService _userIdentityService;
        private readonly IBearerTokenService _bearerTokenService;

        public AccountController(IUserIdentityService userIdentityService, IBearerTokenService bearerTokenService)
        {
            Guard.AgainstNull(userIdentityService);
            Guard.AgainstNull(bearerTokenService);

            _userIdentityService = userIdentityService;
            _bearerTokenService = bearerTokenService;
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(RegisterUserModel userModel) 
        {
            var exisitingUser = _userIdentityService.GetIdentityUser(userModel.Email);
            if (exisitingUser != null)
                return Ok();
            _userIdentityService.Register(userModel.Email, userModel.Password);
            return Ok();
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(UserLoginModel loginModel)
        {
            var exisitingUser = _userIdentityService.GetIdentityUser(loginModel.Email);
            if (exisitingUser == null)
                return BadRequest();

            _userIdentityService.ValidatePassword(loginModel.Email, loginModel.Password);
            return Ok(_bearerTokenService.GetBearerToken(exisitingUser));
        }
    }
}
