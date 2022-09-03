using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.Aggregates.UserAggregate;

namespace Tasker.Application.Repositories
{
    public interface IUserRepository : IRepository<User, int>
    {
    }
}
