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
        public readonly UserStatus Status;
        public readonly string FirstName;
        public readonly string SecondName;

        public TaskWorker(int id, string fname, string lname,UserStatus userStatus) : base(id)
        {
            Status = userStatus;
            Guard.AgainstEmptyOrWhiteSpace(fname);
        }

        protected override void IdentityGuards(int id)
        {
            Guard.AgainstNegative(id);
        }
    }
}
