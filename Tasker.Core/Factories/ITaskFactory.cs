using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Core.Constants;
using TaskAggregate = Tasker.Core.Aggregates.TaskAggregate;

namespace Tasker.Core.Factories
{
    public interface ITaskFactory
    {
        public TaskAggregate.Task RecreateTask(int id, string name, IEnumerable<TaskWorker> taskWorkers, TaskWorker currentWorker, WorkerOrderingScheme orderingScheme, List<TaskHistoryItem> taskHistory, DateTime? createdOn);

        public TaskAggregate.Task CreateNewTask(string name, IEnumerable<TaskWorker> taskWorkers, TaskWorker currentWorker, WorkerOrderingScheme orderingScheme, List<TaskHistoryItem> taskHistory);
    }
}
