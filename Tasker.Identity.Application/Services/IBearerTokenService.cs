using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Identity.Application.Models;

namespace Tasker.Identity.Application.Services
{
    public interface IBearerTokenService
    {
        public string GetBearerToken(IIdentityUser user);
    }
}
