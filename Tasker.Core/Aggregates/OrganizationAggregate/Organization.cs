//using System;
//using System.Collections.Generic;
//using System.Text;
//using Tasker.Core.Abstractions;
//using Tasker.Core.Aggregates.TaskAggregate;
//using Tasker.Core.Helpers;

//namespace Tasker.Core.Aggregates.OrganizationAggregate
//{
//    public class Organization : Entity<int>
//    {
//        public IReadOnlyList<int> Members { get; private set; }

//        private HashSet<int> _members;

//        public Organization(int id, string name, List<int> members = null) : base(id)
//        {
//            Guard.AgainstEmptyOrWhiteSpace(name);
//            if (members == null)
//                _members = new HashSet<int>();
//        }

//        public Organization(string name, List<int> members = null) : this(default(int), name, members) { }
        
//        protected override void IdentityGuards(int id)
//        {
//            Guard.AgainstNegative(id);
//        }
//    }
//}
