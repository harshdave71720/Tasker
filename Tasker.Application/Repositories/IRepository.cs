using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tasker.Core.Abstractions;

namespace Tasker.Application.Repositories
{
    public interface IRepository<TRoot, TKey> where TRoot : Entity<TKey>
    {
        public Task<IEnumerable<TRoot>> GetAll();

        public Task<TRoot> Get(TKey Id);

        public Task<bool> Delete(TKey Id);

        public Task<bool> Save(TRoot item);
    }
}
