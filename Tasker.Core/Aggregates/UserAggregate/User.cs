using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.Abstractions;
using Tasker.Core.Constants;
using Tasker.Core.Helpers;

namespace Tasker.Core.Aggregates.UserAggregate
{
    public class User : Entity<int>
    {
        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string EmailAddress { get; private set; }

        public UserStatus Status { get; set; }

        public WorkerStatus WorkerStatus { get; set; }

        public User
        (
            int id,
            string email,
            string firstname,
            string lastname = null,
            WorkerStatus workerStatus = WorkerStatus.Available
        )
        : base(id)
        {
            Guard.AgainstInvalidEmail(email);
            EmailAddress = email;
            Guard.AgainstEmptyOrWhiteSpace(firstname);
            FirstName = firstname;
            LastName = lastname;
            WorkerStatus = workerStatus;
        }

        public User
        (
            string email,
            string firstname,
            string lastname = null,
            WorkerStatus workerStatus = WorkerStatus.Available
        )
        : this(default(int), email, firstname, lastname, workerStatus) { }

        protected override void IdentityGuards(int id)
        {
            Guard.AgainstNegative(id);
        }
    }
}
