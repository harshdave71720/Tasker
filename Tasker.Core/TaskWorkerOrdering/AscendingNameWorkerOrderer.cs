using System;
using System.Collections.Generic;
using System.Linq;
using Tasker.Core.Aggregates.TaskAggregate;

namespace Tasker.Core.TaskWorkerOrdering
{
    public class AscendingNameWorkerOrderer : WorkerOrderer
    {
        public override void OrderWorkers(IEnumerable<TaskWorker> workers)
        {
            workers = workers.OrderBy(w => w.FirstName.ToUpper()).ThenBy(w => w.LastName?.ToUpper());
            int order = 1;
            foreach (var worker in workers)
            {
                worker.Order = order++;
            }
        }
    }
}
