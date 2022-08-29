using System.Collections.Generic;
using Tasker.Core.Aggregates.TaskAggregate;

namespace Tasker.Core.TaskWorkerOrdering
{
    public abstract class WorkerOrderer
    {
        public abstract void OrderWorkers(IEnumerable<TaskWorker> workers);
    }
}
