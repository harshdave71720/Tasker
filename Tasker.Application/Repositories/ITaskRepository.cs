using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaskAggregate = Tasker.Core.Aggregates.TaskAggregate;

namespace Tasker.Application.Repositories
{
    public interface ITaskRepository : IRepository<TaskAggregate.Task, int>
    {
        public Task<bool> Exists(int id);
    }
}
