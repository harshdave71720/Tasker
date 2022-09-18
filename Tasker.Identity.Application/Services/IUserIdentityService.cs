using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tasker.Identity.Application.Models;

namespace Tasker.Identity.Application.Services
{
    public interface IUserIdentityService
    {
        public IIdentityUser GetIdentityUser(string email);

        public Task<bool> ValidatePassword(string email, string password);

        public void UpdatePassword(string email, string newPassword);

        public Task Register(string email, string password);
    }
}
