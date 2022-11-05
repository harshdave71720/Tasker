using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Identity.Application.Services;
using Tasker.Identity.Application.Models;
using Microsoft.AspNetCore.Identity;
using Tasker.Identity.Infrastructure.Models;
using Tasker.Core.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace Tasker.Identity.Infrastructure.Services
{
    internal class UserIdentityService : IUserIdentityService
    {
        private readonly UserManager<AppIdentityUser> _userManager;

        public UserIdentityService(UserManager<AppIdentityUser> userManager)
        {
            Guard.AgainstNull(userManager);
            _userManager = userManager;
        }

        public IIdentityUser GetIdentityUser(string email)
        {
            return _userManager.Users.SingleOrDefault(u => u.Email == email);
        }

        public async Task Register(string email, string password)
        {
            var result = await _userManager.CreateAsync(new AppIdentityUser(email), password);
            if (!result.Succeeded)
                throw new Exception("Identity User not created");
        }

        public void UpdatePassword(string email, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidatePassword(string email, string password)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Email == email);
            if (user == null)
                return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }
    }
}
