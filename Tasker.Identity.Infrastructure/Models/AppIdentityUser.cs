using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Identity.Application.Models;
using Tasker.Core.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Tasker.Identity.Infrastructure.Models
{
    public class AppIdentityUser : IdentityUser, IIdentityUser
    {
        public AppIdentityUser(string email)
        {
            Guard.AgainstInvalidEmail(email);
            Email = email;
            UserName = email;
        }
    }
}
