using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Identity.Application.Services;
using Tasker.Identity.Application.Models;

namespace Tasker.Identity.Infrastructure.Services
{
    internal class UserIdentityService : IUserIdentityService
    {
        public IIdentityUser GetIdentityUser(string email)
        {
            throw new NotImplementedException();
        }

        public void Register(string email, string firstname, string password)
        {
            throw new NotImplementedException();
        }

        public void UpdatePassword(string email, string newPassword)
        {
            throw new NotImplementedException();
        }

        public bool ValidatePassword(string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}
