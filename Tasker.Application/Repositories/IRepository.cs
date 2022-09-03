using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.Abstractions;

namespace Tasker.Application.Repositories
{
    public interface IRepository<TRoot, TKey> where TRoot : Entity<TKey>
    {
        public IEnumerable<TRoot> GetAll();

        public TRoot Get(TKey Id);

        public bool Delete(TKey Id);

        public bool Save(TRoot item);
    }
}
