using System;
using System.Collections.Generic;
using Tasker.Core.Aggregates.TaskAggregate;

namespace Tasker.Core.TaskWorkerOrdering
{
    public class AscendingNameWorkerOrderer : WorkerOrderer
    {
        public override void OrderWorkers(IEnumerable<TaskWorker> workers)
        {
            throw new NotImplementedException();
        }
    }
}
