using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Identity.Application.Models;
using Tasker.Core.Helpers;

namespace Tasker.Identity.Infrastructure.Models
{
    public class IdentityUser : IIdentityUser
    {
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string Password { get; private set; }

        public IdentityUser(string email, string firstname, string password = null)
        {
            Guard.AgainstInvalidEmail(email);
            Guard.AgainstEmptyOrWhiteSpace(firstname);
        }
    }
}
