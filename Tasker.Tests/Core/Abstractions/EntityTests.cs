using System;
using System.Collections.Generic;
using System.Text;

namespace Tasker.Tests.Core.Abstractions
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Tasker.Core.Abstractions;


    [TestFixture]
    internal class EntityTests
    {
        [Test]
        public void Id_ShouldAllowToChangeIfCurrentIsDefault()
        {
            var sut = new TestEntitySubClass();

            sut.Id = 1;

            Assert.AreEqual(1, sut.Id);
        }

        [Test]
        public void Id_ShouldNotAllowToChangeIfCurrentIsNotDefault()
        {
            var sut = new TestEntitySubClass(1);

            Assert.Throws<InvalidOperationException>(() => { sut.Id = 1; });
        }

        [Test]
        public void Equals_ShouldReturnFalseIfCurrentIdIsDefault()
        {
            var sut = new TestEntitySubClass();
            var other = new TestEntitySubClass();

            Assert.IsFalse(sut.Equals(other));
        }
    }

    class TestEntitySubClass : Entity<int>
    {
        protected override void IdentityGuards(int id)
        {
        }

        public TestEntitySubClass(int id) : base(id) { }

        public TestEntitySubClass() : base(default(int)) { }
    }
}

