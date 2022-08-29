using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.Abstractions;
using Tasker.Core.Constants;

namespace Tasker.Core.Aggregates.TaskAggregate
{
    public class TaskHistoryItem : ValueObject
    {
        public readonly DateTime TimeStamp;

        public readonly int WorkerId;

        public readonly TaskCompletionStatus Status;

        public TaskHistoryItem(DateTime timestamp, int workerId, TaskCompletionStatus status)
        { 
            TimeStamp = timestamp;
            WorkerId = workerId;
            Status = status;
        }

        protected override IEnumerable<object> Members
        {
            get
            {
                yield return WorkerId;
                yield return TimeStamp;
                yield return Status;
            }
        }
    }
}
