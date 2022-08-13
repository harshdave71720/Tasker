using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Tasker.Core.Abstractions
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object> Members { get; }

        public override bool Equals(object obj)
        {
            ValueObject other = obj as ValueObject;
            if (other == null)
                return false;

            if (Members.Count() != other.Members.Count())
                return false;

            var currentEnumerator = Members.GetEnumerator();
            var otherEnumerator = other.Members.GetEnumerator();

            while (currentEnumerator.MoveNext() && otherEnumerator.MoveNext())
            { 
                var currentMember = currentEnumerator.Current;
                var otherMember = otherEnumerator.Current;

                if(currentMember == null && otherMember == null)
                    continue;

                if (currentMember == null)
                    return false;

                if (!currentMember.Equals(otherMember))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int code = GetType().GetHashCode();
            foreach (var member in Members)
            { 
                if(member != null)
                    code = code + member.GetHashCode();
            }

            return code;
        }
    }
}
