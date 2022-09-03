using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.Aggregates.TaskAggregate;

namespace Tasker.Application.Repositories
{
    public interface ITaskRepository : IRepository<Task, int>
    {
    }
}
