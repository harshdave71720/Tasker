using Tasker.Core.Aggregates.UserAggregate;
using Tasker.Core.Aggregates.TaskAggregate;
using TaskAggregate = Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Core.Constants;
using System;
using System.Collections.Generic;
using Tasker.Core.Factories;
using Tasker.Core.Helpers;

namespace Tasker.Infrastructure.Repositories
{
    public class CoreObjectConvertor
    {
        private ITaskFactory _taskFactory;

        public CoreObjectConvertor(ITaskFactory taskFactory)
        {
            Guard.AgainstNull(taskFactory);
            _taskFactory = taskFactory;
        }

        public User ToUser(dynamic user)
        {
            return new User
                (
                    (int)user.Id,
                    (string)user.Email,
                    (string)user.FirstName,
                    (string)user.LastName,
                    (WorkerStatus)user.WorkerStatus
                );
        }

        public TaskWorker ToTaskWorker(dynamic taskWorker)
        {
            return new TaskWorker
                (
                    (int) taskWorker.Id,
                    (string)taskWorker.FirstName,
                    (string)taskWorker.LastName,
                    (WorkerStatus)taskWorker.WorkerStatus
                );
        }

        public TaskHistoryItem ToTaskHistoryItem(dynamic taskHistoryItem)
        {
            return new TaskHistoryItem
                (
                    (DateTime)taskHistoryItem.TimeStamp,
                    (int)taskHistoryItem.WorkerId,
                    (TaskCompletionStatus)taskHistoryItem.Status
                );
        }

        public TaskAggregate.Task ToTask(dynamic task, IEnumerable<TaskWorker> workers, TaskWorker currentWorker, List<TaskHistoryItem> taskHistory)
        {
            return _taskFactory.RecreateTask((int)task.Id, (string)task.Name, workers, currentWorker, (WorkerOrderingScheme)task.WorkerOrderingScheme, taskHistory, (DateTime?) task.CreatedOn);
        }
    }
}
