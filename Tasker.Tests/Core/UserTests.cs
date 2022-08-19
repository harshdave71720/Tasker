using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.Aggregates.UserAggregate;
using Tasker.Core.Constants;

namespace Tasker.Tests.Core
{
    [TestFixture]
    internal class UserTests
    {
        [Test]
        public void Constructor_ShouldThrowExceptionWhenEmailIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new User(1, null, "first", "last"));
        }

        [TestCase("abc.def@mail.c")]
        [TestCase("abc.def@mail#archive.com")]
        [TestCase("abc.def@mail")]
        public void Constructor_ShouldThrowExceptionWhenInvalidEmail(string email)
        {
            Assert.Throws<ArgumentException>(() => new User(1, email, "first", "last"));
        }

        [TestCase("abc-d@mail.com")]
        [TestCase("abc.def@mail.com")]
        [TestCase("abc@mail.com")]
        [TestCase("abc_def@mail.com")]
        [TestCase("abc.def@mail.cc")]
        [TestCase("abc.def@mail-archive.com")]
        [TestCase("abc.def@mail.org")]
        [TestCase("abc.def@mail.com")]
        public void Constructor_ShouldNotThrowExceptionWhenValidEmail(string email)
        {
            Assert.DoesNotThrow(() => new User(1, email, "first", "last"));
        }

        [Test]
        public void Constructor_ShouldThrowExceptionIfFirstNameEmptyOrNull()
        {
            Assert.Throws<ArgumentNullException>(() => { new User(1, "test@org.com", null, "last"); });
            Assert.Throws<ArgumentException>(() => { new User(1, "test@org.com", string.Empty, "last"); });
            Assert.Throws<ArgumentException>(() => { new User(1, "test@org.com", "    ", "last"); });
            Assert.Throws<ArgumentException>(() => { new User(1, "test@org.com", "  \t  \r ", "last"); });
        }

        [Test]
        public void Constructor_ShouldOnlyAllowNonNegativeId()
        {
            Assert.DoesNotThrow(() => { new User(1, "test@org.com", "first", "last"); });
            Assert.DoesNotThrow(() => { new User(100, "test@org.com", "first", "last"); });
            Assert.DoesNotThrow(() => { new User(0, "test@org.com", "first", "last"); });

            Assert.Throws<ArgumentException>(() => { new User(-1, "test@org.com", "first", "last"); });
            Assert.Throws<ArgumentException>(() => { new User(-100, "test@org.com", "first", "last"); });
            Assert.Throws<ArgumentException>(() => { new User(-67, "test@org.com", "first", "last"); });
        }

        [Test]
        public void Contstructor_DefaultsStatusToActive()
        {
            var sut = new User(1, "test@org.com", "first", "last");

            Assert.AreEqual(UserStatus.Active, sut.Status);
        }

        [Test]
        public void Id_ShouldOnlyAllowPositiveToBeSetIfCurrentIsDefault()
        {
            var sut = new User("test@org.com", "first", "last");

            Assert.Throws<ArgumentException>(() => { sut.Id = -1; });
            Assert.Throws<ArgumentException>(() => { sut.Id = -10; });
            Assert.Throws<ArgumentException>(() => { sut.Id = -56; });

            Assert.DoesNotThrow(() => { sut.Id = 1; });
        }

        [TestCase(1)]
        [TestCase(134)]
        [TestCase(179)]
        [TestCase(int.MaxValue)]
        public void GetHashCode_ShouldReturnCorrectValue(int id)
        {
            int expectedHash = id + typeof(User).GetHashCode();
            var sut = new User(id, "test@org.com", "first", "last");

            Assert.AreEqual(expectedHash, sut.GetHashCode());
        }

        [TestCase(1)]
        [TestCase(134)]
        [TestCase(179)]
        [TestCase(int.MaxValue)]
        public void Equals_ShouldReturnTrueWhenIdentityValuesAreSame(int id)
        {
            var sut = new User(id, "test@org.com", "first", "last");
            var other = new User(id, "test@org.com", "first", "last");

            Assert.AreEqual(sut, other);
        }
    }
}
