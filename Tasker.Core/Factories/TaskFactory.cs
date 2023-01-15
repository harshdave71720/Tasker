using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Core.Helpers;
using Tasker.Core.TaskWorkerOrdering.Factories;
using TaskAggregate = Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Core.Constants;
using System.Linq;

namespace Tasker.Core.Factories
{
    public class TaskFactory : ITaskFactory
    {
        private readonly IWorkerOrdererFactory _workerOrdererFactory;

        public TaskFactory(IWorkerOrdererFactory workerOrdererFactory)
        {
            Guard.AgainstNull(workerOrdererFactory);
            _workerOrdererFactory = workerOrdererFactory;
        }

        public TaskAggregate.Task RecreateTask(int id, string name, IEnumerable<TaskWorker> taskWorkers, TaskWorker currentWorker, WorkerOrderingScheme orderingScheme, List<TaskHistoryItem> taskHistory, DateTime? createdOn)
        {
            return new TaskAggregate.Task(id, name, new WorkerPool(taskWorkers.ToHashSet(), _workerOrdererFactory, orderingScheme), currentWorker, orderingScheme, taskHistory, createdOn);
        }

        public TaskAggregate.Task CreateNewTask(string name, IEnumerable<TaskWorker> taskWorkers, TaskWorker currentWorker, WorkerOrderingScheme orderingScheme, List<TaskHistoryItem> taskHistory)
        {
            return new TaskAggregate.Task(name, new WorkerPool(taskWorkers.ToHashSet(), _workerOrdererFactory, orderingScheme), currentWorker, orderingScheme);
        }
    }
}
