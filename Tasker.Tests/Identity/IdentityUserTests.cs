using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tasker.Identity.Infrastructure.Models;

namespace Tasker.Tests.Identity
{
    [TestFixture]
    internal class IdentityUserTests
    {
        [Test]
        public void Constructor_ShouldThrowExceptionIfEmailIsNotValid()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityUser(null, "firstname"));
            Assert.Throws<ArgumentException>(() => new IdentityUser(string.Empty, "firstname"));
            Assert.Throws<ArgumentException>(() => new IdentityUser("", "firstname"));
            Assert.Throws<ArgumentException>(() => new IdentityUser("          ", "firstname"));
            Assert.Throws<ArgumentException>(() => new IdentityUser("       \t \r   ", "firstname"));
            Assert.Throws<ArgumentException>(() => new IdentityUser("    haarhesr   ", "firstname"));
            Assert.Throws<ArgumentException>(() => new IdentityUser("abc@rt", "firstname"));
        }

        [Test]
        public void Constructor_ShouldThrowExceptionIfFirstnameIsNotValid()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityUser("test@test.com", null));
            Assert.Throws<ArgumentException>(() => new IdentityUser("test@test.com", string.Empty));
            Assert.Throws<ArgumentException>(() => new IdentityUser("test@test.com", ""));
            Assert.Throws<ArgumentException>(() => new IdentityUser("test@test.com", "    "));
            Assert.Throws<ArgumentException>(() => new IdentityUser("test@test.com", "  \r \t "));
        }
    }
}
