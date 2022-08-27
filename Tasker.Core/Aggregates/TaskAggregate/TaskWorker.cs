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
        private int _order;
        public int Order 
        { 
            get { return _order;  }
            set 
            {
                Guard.AgainstNegative(value);
                _order = value;
            }
        }

        public TaskWorker(int id, string fname, string lname, WorkerStatus workerStatus, int order = -1) : base(id)
        {
            Status = workerStatus;
            Guard.AgainstEmptyOrWhiteSpace(fname);
            FirstName = fname;
            LastName = lname;
            _order = order;
        }

        protected override void IdentityGuards(int id)
        {
            Guard.AgainstNegative(id);
        }
    }
}
