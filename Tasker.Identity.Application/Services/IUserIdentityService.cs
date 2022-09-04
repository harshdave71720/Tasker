using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Identity.Application.Models;

namespace Tasker.Identity.Application.Services
{
    public interface IUserIdentityService
    {
        public IIdentityUser GetIdentityUser(string email);

        public bool ValidatePassword(string email, string password);

        public void UpdatePassword(string email, string newPassword);

        public void Register(string email, string firstname, string password);
    }
}
