using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tasker.Identity.Infrastructure.Models;

namespace Tasker.Tests.Identity
{
    [TestFixture]
    internal class AppIdentityUserTests
    {
        [Test]
        public void Constructor_ShouldThrowExceptionIfEmailIsNotValid()
        {
            Assert.Throws<ArgumentNullException>(() => new AppIdentityUser(null));
            Assert.Throws<ArgumentException>(() => new AppIdentityUser(string.Empty));
            Assert.Throws<ArgumentException>(() => new AppIdentityUser(""));
            Assert.Throws<ArgumentException>(() => new AppIdentityUser("          "));
            Assert.Throws<ArgumentException>(() => new AppIdentityUser("       \t \r   "));
            Assert.Throws<ArgumentException>(() => new AppIdentityUser("    haarhesr   "));
            Assert.Throws<ArgumentException>(() => new AppIdentityUser("abc@rt"));
        }
        
        //[Test]
        //public void Constructor_ShouldThrowExceptionIfEmailIsNotValid()
        //{
        //    Assert.Throws<ArgumentNullException>(() => new AppIdentityUser(null, "firstname"));
        //    Assert.Throws<ArgumentException>(() => new AppIdentityUser(string.Empty, "firstname"));
        //    Assert.Throws<ArgumentException>(() => new AppIdentityUser("", "firstname"));
        //    Assert.Throws<ArgumentException>(() => new AppIdentityUser("          ", "firstname"));
        //    Assert.Throws<ArgumentException>(() => new AppIdentityUser("       \t \r   ", "firstname"));
        //    Assert.Throws<ArgumentException>(() => new AppIdentityUser("    haarhesr   ", "firstname"));
        //    Assert.Throws<ArgumentException>(() => new AppIdentityUser("abc@rt", "firstname"));
        //}

        //[Test]
        //public void Constructor_ShouldThrowExceptionIfFirstnameIsNotValid()
        //{
        //    Assert.Throws<ArgumentNullException>(() => new AppIdentityUser("test@test.com", null));
        //    Assert.Throws<ArgumentException>(() => new AppIdentityUser("test@test.com", string.Empty));
        //    Assert.Throws<ArgumentException>(() => new AppIdentityUser("test@test.com", ""));
        //    Assert.Throws<ArgumentException>(() => new AppIdentityUser("test@test.com", "    "));
        //    Assert.Throws<ArgumentException>(() => new AppIdentityUser("test@test.com", "  \r \t "));
        //}
    }
}
