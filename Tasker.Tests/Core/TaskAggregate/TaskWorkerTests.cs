using NUnit.Framework;
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
            Assert.Throws<ArgumentException>(() => { new TaskWorker(-1, "Jane", "Doe", WorkerStatus.Available); });
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        [TestCase(1236)]
        public void GetHashCode_ReturnsCorrectValue(int id)
        {
            var expected = typeof(TaskWorker).GetHashCode() + id;
            Assert.AreEqual(expected, new TaskWorker(id, "Jane", "Doe", WorkerStatus.Available).GetHashCode());
        }

        [Test]
        public void Constructor_ShouldThrowExceptionWhenFirstNameIsEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => { new TaskWorker(1, null, "Doe", WorkerStatus.Available); });
            Assert.Throws<ArgumentException>(() => { new TaskWorker(1, String.Empty, "Doe", WorkerStatus.Available); });
            Assert.Throws<ArgumentException>(() => { new TaskWorker(1, "   ", "Doe", WorkerStatus.Available); });
            Assert.Throws<ArgumentException>(() => { new TaskWorker(1, "   \t \r ", "Doe", WorkerStatus.Available); });
        }

        [Test]
        public void Equals_ShouldReturnFalseWhenIdsAreDefaultOrNotMatching()
        {
            Assert.False(new TaskWorker(0, "Jane", "Doe", WorkerStatus.Available).Equals(new TaskWorker(0, "Jane", "Doe", WorkerStatus.Available)));
            Assert.False(new TaskWorker(1, "Jane", "Doe", WorkerStatus.Available).Equals(new TaskWorker(0, "Jane", "Doe", WorkerStatus.Available)));
            Assert.False(new TaskWorker(0, "Jane", "Doe", WorkerStatus.Available).Equals(new TaskWorker(1, "Jane", "Doe", WorkerStatus.Available)));
            Assert.False(new TaskWorker(12, "Jane", "Doe", WorkerStatus.Available).Equals(new TaskWorker(21, "Jane", "Doe", WorkerStatus.Available)));
        }

        [Test]
        public void Equals_ShouldReturnTrueWhenIdsAreSame()
        {
            Assert.True(new TaskWorker(1, "Jane", "Doe", WorkerStatus.Available).Equals(new TaskWorker(1, "Jane", "Doe", WorkerStatus.Available)));
            Assert.True(new TaskWorker(12, "Jane", "Doe", WorkerStatus.Available).Equals(new TaskWorker(12, "Jane", "Doe", WorkerStatus.Available)));
        }
    }
}
