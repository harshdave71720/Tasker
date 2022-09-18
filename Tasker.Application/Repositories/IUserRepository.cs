using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tasker.Core.Aggregates.UserAggregate;

namespace Tasker.Application.Repositories
{
    public interface IUserRepository : IRepository<User, int>
    {
        Task<User> Get(string email);
    }
}
