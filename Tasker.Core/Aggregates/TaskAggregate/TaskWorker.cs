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

        public TaskWorker(int id, UserStatus userStatus) : base(id)
        {
            Status = userStatus;
        }

        protected override void IdentityGuards(int id)
        {
            Guard.AgainstNegative(id);
        }
    }
}
