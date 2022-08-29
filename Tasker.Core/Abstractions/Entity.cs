using System;
using System.Collections.Generic;
using System.Text;

namespace Tasker.Core.Abstractions
{
    public abstract class Entity<T>
    {
        public Entity(T id)
        {
            IdentityGuards(id);
            _id = id;
        }

        private T _id;

        public T Id 
        {
            get { return _id; }
            set
            {
                if (!_id.Equals(default(T)) || value.Equals(default(T)))
                    throw new InvalidOperationException();

                IdentityGuards(value);
                _id = value;
            }
        }

        protected abstract void IdentityGuards(T id);

        public override int GetHashCode()
        {
            return GetType().GetHashCode() + Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this.Id.Equals(default(T)))
                return false;

            if (obj == null)
                return false;

            Entity<T> other = obj as Entity<T>;
            if (other == null)
                return false;

            return ReferenceEquals(this, other) || other.Id.Equals(Id);
        }
    }
}
