using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Application.Repositories;
using Tasker.Core.Aggregates.UserAggregate;
using Tasker.Infrastructure.Settings;

namespace Tasker.Infrastructure.Repositories
{
    internal class UserRepository : DapperRepository, IUserRepository
    {
        public UserRepository(IOptions<DataStoreSettings> settingOptions) : base(settingOptions) { }

        public bool Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public User Get(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public bool Save(User item)
        {
            throw new NotImplementedException();
        }
    }
}
