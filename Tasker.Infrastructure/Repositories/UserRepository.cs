using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tasker.Application.Repositories;
using Tasker.Core.Aggregates.UserAggregate;
using Tasker.Infrastructure.Settings;

namespace Tasker.Infrastructure.Repositories
{
    internal class UserRepository : DapperRepository, IUserRepository
    {
        public UserRepository(IOptions<DataStoreSettings> settingOptions) : base(settingOptions) { }

        public Task<bool> Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<User> Get(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Save(User item)
        {
            throw new NotImplementedException();
        }
    }
}
