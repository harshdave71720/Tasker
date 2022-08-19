﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Core.Constants;

namespace Tasker.Tests.Core.TaskAggregate
{
    [TestFixture]
    internal class TaskWorkerTests
    {
        [Test]
        public void Constructor_ShouldThrowExceptionWhenIdIsNegative()
        {
            Assert.Throws<ArgumentException>(() => { new TaskWorker(-1, UserStatus.Active); });
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        [TestCase(1236)]
        public void GetHashCode_ReturnsCorrectValue(int id)
        {
            var expected = typeof(TaskWorker).GetHashCode() + id;
            Assert.AreEqual(expected, new TaskWorker(id, UserStatus.Active).GetHashCode());
        }

        [Test]
        public void Equals_ShouldReturnFalseWhenIdsAreDefaultOrNotMatching()
        {
            Assert.False(new TaskWorker(0, UserStatus.Active).Equals(new TaskWorker(0, UserStatus.Active)));
            Assert.False(new TaskWorker(1, UserStatus.Active).Equals(new TaskWorker(0, UserStatus.Active)));
            Assert.False(new TaskWorker(0, UserStatus.Active).Equals(new TaskWorker(1, UserStatus.Active)));
            Assert.False(new TaskWorker(12, UserStatus.Active).Equals(new TaskWorker(21, UserStatus.Active)));
        }

        [Test]
        public void Equals_ShouldReturnTrueWhenIdsAreSame()
        {
            Assert.True(new TaskWorker(1, UserStatus.Active).Equals(new TaskWorker(1, UserStatus.Active)));
            Assert.True(new TaskWorker(12, UserStatus.Active).Equals(new TaskWorker(12, UserStatus.Active)));
        }
    }
}
