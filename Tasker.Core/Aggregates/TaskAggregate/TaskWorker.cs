using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.Abstractions;
using Tasker.Core.Constants;
using Tasker.Core.Helpers;

namespace Tasker.Core.Aggregates.TaskAggregate
{
    public class TaskWorker : Entity<int>
    {
        public readonly WorkerStatus Status;
        public readonly string FirstName;
        public readonly string LastName;

        public TaskWorker(int id, string fname, string lname, WorkerStatus workerStatus) : base(id)
        {
            Status = workerStatus;
            Guard.AgainstEmptyOrWhiteSpace(fname);
            FirstName = fname;
            LastName = lname;
        }

        protected override void IdentityGuards(int id)
        {
            Guard.AgainstNegative(id);
        }
    }
}
