using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Core.Constants;
using Moq;
using Tasker.Core.TaskWorkerOrdering;
using Tasker.Core.TaskWorkerOrdering.Factories;

namespace Tasker.Tests.Core.TaskAggregate
{
    [TestFixture]
    internal class TaskTests
    {
        private TaskWorker SampleTaskWorker => new TaskWorker(1, "Jane", "Doe", WorkerStatus.Available);

        [Test]
        public void Constructor_ShouldThrowExceptionIfNameIsInvalid()
        {
            var workers = CreateWorkerPool(SampleTaskWorker);
            Assert.Throws<ArgumentNullException>(() => new Task(null, workers, SampleTaskWorker));
            Assert.Throws<ArgumentException>(() => new Task(String.Empty, workers, SampleTaskWorker));
            Assert.Throws<ArgumentException>(() => new Task("", workers, SampleTaskWorker));
            Assert.Throws<ArgumentException>(() => new Task("     ", workers, SampleTaskWorker));
            Assert.Throws<ArgumentException>(() => new Task("     \r \t    ", workers, SampleTaskWorker));
        }

        [Test]
        public void Constructor_ShouldThrowExceptionIfWorkersAreNullOrEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new Task("Task1", null, SampleTaskWorker));
            Assert.Throws<ArgumentException>(() => new Task("Task2", CreateWorkerPool(), SampleTaskWorker));
        }

        [Test]
        public void Constructor_ShouldInitializeEmptyHistoryIfGivenNull()
        {
            var sut = new Task("Task1", CreateWorkerPool(SampleTaskWorker), SampleTaskWorker);

            Assert.IsNotNull(sut.History);
            Assert.IsEmpty(sut.History);
        }

        [Test]
        public void Constructor_ShouldThrowExceptionIfIdIsNegative()
        {
            Assert.Throws<ArgumentException>(() => new Task(-1, "Task2", CreateWorkerPool(SampleTaskWorker), SampleTaskWorker));
        }

        [Test]
        public void Constructor_ShouldThrowExceptionIfCurrentWorkerIsAbsent()
        {
            var currentWorker = new TaskWorker(1, "jane", "Doe", WorkerStatus.Absent);
            Assert.Throws<InvalidOperationException>(() => new Task(1, "Task2", CreateWorkerPool(currentWorker), currentWorker));
        }

        [Test]
        public void Constructor_ShouldThrowExceptionIfCurrentWorkerIsNotInWorkersList()
        {
            var currentWorker = new TaskWorker(new Random().Next(2, int.MaxValue), "jane", "Doe", WorkerStatus.Available);
            Assert.Throws<InvalidOperationException>(() => new Task(1, "Task2", CreateWorkerPool(SampleTaskWorker), currentWorker));
        }

        [Test]
        public void MarkDone_ShouldThrowExceptionIfCurrentWorkerIsNull()
        {
            var sut = new Task("Task1", CreateWorkerPool(SampleTaskWorker), currentWorker: null);

            Assert.Throws<InvalidOperationException>(() => sut.MarkDone());
        }

        [Test]
        public void MarkDone_ShouldNewItemToHistoryWithCorrectDetails()
        {
            var worker = SampleTaskWorker;
            var sut = new Task("Task1", CreateWorkerPool(SampleTaskWorker), currentWorker: worker);

            sut.MarkDone();
            var latestHistoryItem = sut.History.Last();

            Assert.AreEqual(sut.History.Count(), 1);
            Assert.AreEqual(worker.Id, latestHistoryItem.WorkerId);
            Assert.AreEqual(TaskCompletionStatus.Done, latestHistoryItem.Status);
        }

        [Test]
        public void MarkDone_ShouldUpdateCurrentWorker()
        {
            var worker = SampleTaskWorker;
            var sut = new Task("Task1", CreateWorkerPool(worker, new TaskWorker(2, "worker2", null, WorkerStatus.Available)), currentWorker: worker);

            sut.MarkDone();
            Assert.AreNotEqual(worker, sut.CurrentWorker);
        }

        [Test]
        public void Skip_ShouldThrowExceptionIfCurrentWorkerIsNull()
        {
            var sut = new Task("Task1", CreateWorkerPool(SampleTaskWorker), currentWorker: null);

            Assert.Throws<InvalidOperationException>(() => sut.Skip());
        }

        [Test]
        public void Skip_ShouldNewItemToHistoryWithCorrectDetails()
        {
            var worker = SampleTaskWorker;
            var sut = new Task("Task1", CreateWorkerPool(worker), currentWorker: worker);

            sut.Skip();
            var latestHistoryItem = sut.History.Last();

            Assert.AreEqual(sut.History.Count(), 1);
            Assert.AreEqual(worker.Id, latestHistoryItem.WorkerId);
            Assert.AreEqual(TaskCompletionStatus.Skipped, latestHistoryItem.Status);
        }

        [Test]
        public void Skip_ShouldUpdateCurrentWorker()
        {
            var worker = SampleTaskWorker;
            var sut = new Task("Task1", CreateWorkerPool(worker, new TaskWorker(2, "worker2", null, WorkerStatus.Available)), currentWorker: worker);

            sut.Skip();
            Assert.AreNotEqual(worker, sut.CurrentWorker);
        }

        [TestCase(1)]
        [TestCase(1000)]
        [TestCase(45353)]
        [TestCase(int.MaxValue)]
        public void GetHashCode_ShouldReturnCorrectValue(int id)
        {
            var expectedHashCode = typeof(Task).GetHashCode() + id.GetHashCode();
            var sut = new Task(id, "Task1", CreateWorkerPool(SampleTaskWorker), SampleTaskWorker);

            Assert.AreEqual(expectedHashCode, sut.GetHashCode());
        }

        [Test]
        public void Equals_ShouldReturnFalseIfAnyTaskHasDefaultIdentityValue()
        {
            var task1 = new Task("Task1", CreateWorkerPool(SampleTaskWorker), SampleTaskWorker);
            var task2 = new Task("Task1", CreateWorkerPool(SampleTaskWorker), SampleTaskWorker);

            Assert.False(task1.Equals(task2));
        }

        [TestCase(1)]
        [TestCase(100)]
        [TestCase(145346)]
        [TestCase(int.MaxValue)]
        public void Equals_ShouldReturnTrueIfIdentitiesAreEqual(int id)
        {
            var task1 = new Task(id, "Task1", CreateWorkerPool(SampleTaskWorker), SampleTaskWorker);
            var task2 = new Task(id, "Task1", CreateWorkerPool(SampleTaskWorker), SampleTaskWorker);
            var task3 = new Task(4234234, "Task1", CreateWorkerPool(SampleTaskWorker), SampleTaskWorker);

            Assert.True(task1.Equals(task2));
            Assert.False(task1.Equals(task3));
        }

        [Test]
        public void AddWorker_ShouldAddNewWorker()
        {
            var workerToAdd = new TaskWorker(100, "Jane", "Doe", WorkerStatus.Available);
            var sut = new Task("TestTask", CreateWorkerPool(new TaskWorker(12, "Jane", "Doe", WorkerStatus.Available)), null);

            sut.AddWorker(workerToAdd);

            Assert.AreEqual(2, sut.PossibleWorkers.Count());
        }

        [Test]
        public void AddWorker_ShouldDoNothingIfWorkerAlreadyPresent()
        {
            var workerToAdd = new TaskWorker(100, "Jane", "Doe", WorkerStatus.Available);
            var sut = new Task("TestTask", CreateWorkerPool(new TaskWorker(workerToAdd.Id, "Jane", "Doe", WorkerStatus.Available)));

            sut.AddWorker(workerToAdd);

            Assert.IsNotNull(sut.PossibleWorkers.Single(w => w.Equals(workerToAdd)));
        }

        //[Test]
        //public void AddWorker_ShouldReorderWorkersIfWorkerAdded()
        //{
        //    var workerToAdd = new TaskWorker(100, "Jane", "Doe", WorkerStatus.Available);
        //    var sut = new Task("TestTask", CreateWorkerPool(new TaskWorker(4, "Jane", "Doe", WorkerStatus.Available)));
        //    var orderer = new Mock<WorkerOrderer>();

        //    sut.AddWorker(workerToAdd);

        //    orderer.Verify(x => x.OrderWorkers(sut.PossibleWorkers), Times.Once);
        //}

        //[Test]
        //public void AddWorker_ShouldNotReorderWorkersIfWorkerWasAlreadyPresent()
        //{
        //    var workerToAdd = new TaskWorker(100, "Jane", "Doe", WorkerStatus.Available);
        //    var sut = new Task("TestTask", CreateWorkerPool(new TaskWorker(workerToAdd.Id, "Jane", "Doe", WorkerStatus.Available)));
        //    var orderer = new Mock<WorkerOrderer>();

        //    sut.AddWorker(workerToAdd);

        //    orderer.Verify(x => x.OrderWorkers(sut.PossibleWorkers), Times.Never);
        //}

        //[Test]
        //public void RemoveWorker_ShouldRemoveWorker()
        //{
        //    var workerToRemove = new TaskWorker(10, "Jane", "Doe", WorkerStatus.Available);
        //    var sut = new Task("TestTask", CreateWorkerPool(new TaskWorker(workerToRemove.Id, "Jane", "Doe", WorkerStatus.Available), SampleTaskWorker));

        //    sut.RemoveWorker(workerToRemove.Id);

        //    Assert.False(sut.PossibleWorkers.Any(w => w.Id == workerToRemove.Id));
        //}

        //[Test]
        //public void RemoveWorker_ShouldDoNothingIfWorkerNotAssigned()
        //{
        //    var workerToRemove = new TaskWorker(10, "Jane", "Doe", WorkerStatus.Available);
        //    var sut = new Task("TestTask", CreateWorkerPool(SampleTaskWorker));

        //    sut.RemoveWorker(workerToRemove.Id);
        //    Assert.AreEqual(1, sut.PossibleWorkers.Count());
        //}

        [Test]
        public void RemoveWorker_ShouldThrowExceptionIfCurrentWorkerIsPassed()
        {
            var workerToRemove = new TaskWorker(10, "Jane", "Doe", WorkerStatus.Available);
            var sut = new Task("TestTask", CreateWorkerPool(SampleTaskWorker, new TaskWorker(workerToRemove.Id, "Jane", "Doe", WorkerStatus.Absent)), workerToRemove);

            Assert.Throws<InvalidOperationException>(() => sut.RemoveWorker(workerToRemove.Id));
        }

        private WorkerPool CreateWorkerPool(params TaskWorker[] workers)
        {
            var orderer = new Mock<WorkerOrderer>();
            SetupSerialOrdering(orderer);
            var ordererFactory = new Mock<IWorkerOrdererFactory>();
            ordererFactory.Setup(f => f.CreateOrderer(It.IsAny<WorkerOrderingScheme>())).Returns(orderer.Object);
            return new WorkerPool(workers.ToList(), ordererFactory.Object, WorkerOrderingScheme.AscendingNameScheme);
        }

        private void SetupSerialOrdering(Mock<WorkerOrderer> orderer)
        {
            orderer.Setup(o => o.OrderWorkers(It.IsAny<IEnumerable<TaskWorker>>()))
                .Callback((IEnumerable<TaskWorker> workers) =>
                {
                    int i = 1;
                    foreach (var worker in workers)
                    {
                        worker.Order = i++;
                    }
                });
        }
    }
}
