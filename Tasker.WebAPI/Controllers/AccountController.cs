using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Transactions;
using Tasker.Application.Repositories;
using Tasker.Core.Aggregates.UserAggregate;
using Tasker.Core.Helpers;
using Tasker.Identity.Application.Services;
using Tasker.WebAPI.Models;

namespace Tasker.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IUserIdentityService _userIdentityService;
        private readonly IBearerTokenService _bearerTokenService;
        private readonly IUserRepository _userRepository;

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
            using (TransactionScope scope = new TransactionScope())
            {
                _userIdentityService.Register(userModel.Email, userModel.Password);
                _userRepository.Save(new User(userModel.Email, userModel.FirstName, userModel.LastName));
                scope.Complete();
            }
            return Ok();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserLoginModel loginModel)
        {
            var exisitingUser = _userIdentityService.GetIdentityUser(loginModel.Email);
            if (exisitingUser == null)
                return BadRequest();

            var userValid = await _userIdentityService.ValidatePassword(loginModel.Email, loginModel.Password);
            if(!userValid)
                return BadRequest();

            return Ok(_bearerTokenService.GetBearerToken(exisitingUser));
        }
    }
}
