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

        public AccountController(IUserIdentityService userIdentityService, IBearerTokenService bearerTokenService, IUserRepository userRepository)
        {
            Guard.AgainstNull(userIdentityService);
            Guard.AgainstNull(bearerTokenService);
            Guard.AgainstNull(userRepository);

            _userIdentityService = userIdentityService;
            _bearerTokenService = bearerTokenService;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterUserModel userModel)
        {
            var exisitingUser = _userIdentityService.GetIdentityUser(userModel.Email);
            if (exisitingUser != null)
                return BadRequest();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _userIdentityService.Register(userModel.Email, userModel.Password);
                var user = await _userRepository.Save(new User(userModel.Email, userModel.FirstName, userModel.LastName));
                scope.Complete();
                return Ok(new Response<User>(user));
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserLoginModel loginModel)
        {
            var exisitingUser = _userIdentityService.GetIdentityUser(loginModel.Email);
            var appUser = await _userRepository.Get(loginModel.Email);
            if (exisitingUser == null || appUser == null)
                return BadRequest();

            var passwordValid = await _userIdentityService.ValidatePassword(loginModel.Email, loginModel.Password);
            if(!passwordValid)
                return BadRequest();

            //return Ok(_bearerTokenService.GetBearerToken(exisitingUser));
            return Ok(new Response<AuthenticatedUserModel>
                (
                    new AuthenticatedUserModel()
                    { 
                        Email = appUser.EmailAddress, 
                        FirstName = appUser.FirstName,
                        LastName = appUser.LastName,
                        Token = _bearerTokenService.GetBearerToken(exisitingUser)
                    }
                ));
        }
    }
}
