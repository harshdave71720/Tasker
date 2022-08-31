using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Core.TaskWorkerOrdering;
using Tasker.Core.Constants;

namespace Tasker.Tests.Core.TaskWorkerOrdering
{
    [TestFixture]
    internal class AscendingNameWorkerOrdererTests
    {
        [Test]
        public void OrderWorkers_ShouldAssignOrderAccordingToFirstNames()
        {
            var sut = new AscendingNameWorkerOrderer();
            var workers = new List<TaskWorker>()
            {
                new TaskWorker(1, "cbc", "lname", WorkerStatus.Available),
                new TaskWorker(1, "abc", "lname", WorkerStatus.Available),
                new TaskWorker(1, "abd", "lname", WorkerStatus.Available),
            };

            sut.OrderWorkers(workers);

            Assert.AreEqual(3, workers[0].Order);
            Assert.AreEqual(1, workers[1].Order);
            Assert.AreEqual(2, workers[2].Order);
        }

        [Test]
        public void OrderWorkers_ShouldIgnoreLetterCaseInFirstName()
        {
            var sut = new AscendingNameWorkerOrderer();
            var workers = new List<TaskWorker>()
            {
                new TaskWorker(1, "ABC", "lname", WorkerStatus.Available),
                new TaskWorker(1, "abc", "lname", WorkerStatus.Available)
            };

            sut.OrderWorkers(workers);

            Assert.AreEqual(1, workers[0].Order);
            Assert.AreEqual(2, workers[1].Order);
        }

        [Test]
        public void OrderWorkers_ShouldIgnoreLetterCaseInLastName()
        {
            var sut = new AscendingNameWorkerOrderer();
            var workers = new List<TaskWorker>()
            {
                new TaskWorker(1, "ABC", "LNAME", WorkerStatus.Available),
                new TaskWorker(1, "abc", "lname", WorkerStatus.Available),
                new TaskWorker(1, "abc", null, WorkerStatus.Available),
            };

            sut.OrderWorkers(workers);

            Assert.AreEqual(2, workers[0].Order);
            Assert.AreEqual(3, workers[1].Order);
            Assert.AreEqual(1, workers[2].Order);
        }

        [Test]
        public void OrderWorkers_ShouldCompareLastNameIfFirstNamesAreSame()
        {
            var sut = new AscendingNameWorkerOrderer();
            var workers = new List<TaskWorker>()
            {
                new TaskWorker(1, "ABC", "blname", WorkerStatus.Available),
                new TaskWorker(1, "abc", "aname", WorkerStatus.Available),
                new TaskWorker(1, "abc", null, WorkerStatus.Available),
            };

            sut.OrderWorkers(workers);

            Assert.AreEqual(3, workers[0].Order);
            Assert.AreEqual(2, workers[1].Order);
            Assert.AreEqual(1, workers[2].Order);
        }
    }
}
