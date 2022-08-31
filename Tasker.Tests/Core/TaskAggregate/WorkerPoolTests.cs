using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.Aggregates.TaskAggregate;
using Moq;
using Tasker.Core.TaskWorkerOrdering;
using Tasker.Core.Constants;
using Tasker.Core.TaskWorkerOrdering.Factories;

namespace Tasker.Tests.Core.TaskAggregate
{
    [TestFixture]
    internal class WorkerPoolTests
    {
        private TaskWorker SampleTaskWorker => new TaskWorker(1, "Jane", "Doe", WorkerStatus.Available);
            
        [Test]
        public void Constructor_ShouldThrowExceptionIfWorkersIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new WorkerPool(null, new Mock<IWorkerOrdererFactory>().Object, WorkerOrderingScheme.None));
        }

        [Test]
        public void Constructor_ShouldThrowExceptionIfOrdererFactoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new WorkerPool(new List<TaskWorker>() { SampleTaskWorker }, null, WorkerOrderingScheme.None));
        }

        [Test]
        public void Constructor_ShouldOrderWorkersViaOrderer()
        {
            var workers = new List<TaskWorker>() { SampleTaskWorker };
            var orderer = new Mock<WorkerOrderer>();
            var ordererFactory = new Mock<IWorkerOrdererFactory>();
            var ob = ordererFactory.Object;
            ordererFactory.Setup(f => f.CreateOrderer(It.IsAny<WorkerOrderingScheme>())).Returns(orderer.Object);
            var sut = new WorkerPool(workers, ordererFactory.Object, WorkerOrderingScheme.AscendingNameScheme);

            ordererFactory.Verify(o => o.CreateOrderer(WorkerOrderingScheme.AscendingNameScheme), Times.Once);
            orderer.Verify(o => o.OrderWorkers(workers), Times.Once);
        }

        [Test]
        public void GetNextWorker_ShouldThrowExceptionWhenCurrentIsNull()
        {
            var workers = new List<TaskWorker>() { SampleTaskWorker };
            var orderer = new Mock<WorkerOrderer>();
            var ordererFactory = new Mock<IWorkerOrdererFactory>();
            ordererFactory.Setup(f => f.CreateOrderer(It.IsAny<WorkerOrderingScheme>())).Returns(orderer.Object);
            var sut = new WorkerPool(workers, ordererFactory.Object, WorkerOrderingScheme.AscendingNameScheme);

            Assert.Throws<ArgumentNullException>(() => sut.GetNextWorker(null));
        }

        [TestCase(1, 2, 4)]
        [TestCase(2, 2, 4)]
        [TestCase(1, 4, 4)]
        [TestCase(4, 1, 4)]
        [TestCase(4, 3, 4)]
        public void GetNextWorker_ShouldReturnNextAvailableWorkerPresent(int currentWorkerId, int nextActiveWorkerId, int totalWorkers)
        {
            // Arrange
            var orderNumbersToMarkAbsent = new HashSet<int>();
            var currentWorkerOrder = currentWorkerId;
            var nextActiveWorkerOrder = nextActiveWorkerId;
            int next = currentWorkerOrder + 1 <= totalWorkers ? currentWorkerOrder + 1 : 1;
            while (next != nextActiveWorkerOrder)
            {
                orderNumbersToMarkAbsent.Add(next);
                next = next + 1 <= totalWorkers ? next + 1 : 1;
            }

            List<TaskWorker> workers = new List<TaskWorker>();
            for (int i = 1; i <= totalWorkers; i++)
            {
                workers.Add(new TaskWorker(i, "Worker" + i, "lname" + i, orderNumbersToMarkAbsent.Contains(i) ? WorkerStatus.Absent : WorkerStatus.Available));
            }

            var orderer = new Mock<WorkerOrderer>();
            orderer.Setup(o => o.OrderWorkers(workers)).Callback((IEnumerable<TaskWorker> workers) =>
            {
                foreach (var worker in workers)
                {
                    worker.Order = worker.Id;
                }
            });

            var ordererFactory = new Mock<IWorkerOrdererFactory>();
            ordererFactory.Setup(f => f.CreateOrderer(It.IsAny<WorkerOrderingScheme>())).Returns(orderer.Object);
            var sut = new WorkerPool(workers, ordererFactory.Object, WorkerOrderingScheme.AscendingNameScheme);

            // Act and Assert
            Assert.AreEqual(workers[nextActiveWorkerId - 1], sut.GetNextWorker(new TaskWorker(currentWorkerId, "dfs", "fds", WorkerStatus.Available)));
        }

        [Test]
        public void GetNextWorker_ShouldReturnNullIfNoNextWorkerAvailable()
        {
            // Arrange
            List<TaskWorker> workers = new List<TaskWorker>();
            for (int i = 1; i <= 10; i++)
            {
                workers.Add(new TaskWorker(i, "Worker" + i, "lname" + i, WorkerStatus.Absent));
            }

            var orderer = new Mock<WorkerOrderer>();
            orderer.Setup(o => o.OrderWorkers(workers)).Callback((IEnumerable<TaskWorker> workers) =>
            {
                foreach (var worker in workers)
                {
                    worker.Order = worker.Id;
                }
            });

            var ordererFactory = new Mock<IWorkerOrdererFactory>();
            ordererFactory.Setup(f => f.CreateOrderer(It.IsAny<WorkerOrderingScheme>())).Returns(orderer.Object);
            var sut = new WorkerPool(workers, ordererFactory.Object, WorkerOrderingScheme.AscendingNameScheme);

            // Act and Assert
            Assert.IsNull(sut.GetNextWorker(workers[0]));
        }

        [Test]
        public void AddWorker_ShuldThrowExceptionIfWorkerIsNull()
        {
            var workers = new List<TaskWorker>() { SampleTaskWorker };
            var orderer = new Mock<WorkerOrderer>();
            var ordererFactory = new Mock<IWorkerOrdererFactory>();
            ordererFactory.Setup(f => f.CreateOrderer(It.IsAny<WorkerOrderingScheme>())).Returns(orderer.Object);
            var sut = new WorkerPool(workers, ordererFactory.Object, WorkerOrderingScheme.AscendingNameScheme);

            Assert.Throws<ArgumentNullException>(() => sut.AddWorker(null));
        }

        [Test]
        public void AddWorker_ShouldAddWorkerIfNotExists_AndReorderWorkers()
        {
            var workers = new List<TaskWorker>() { SampleTaskWorker };
            var orderer = new Mock<WorkerOrderer>();
            SetupSerialOrdering(orderer);
            var ordererFactory = new Mock<IWorkerOrdererFactory>();
            ordererFactory.Setup(f => f.CreateOrderer(It.IsAny<WorkerOrderingScheme>())).Returns(orderer.Object);
            var sut = new WorkerPool(workers, ordererFactory.Object, WorkerOrderingScheme.AscendingNameScheme);

            sut.AddWorker(new TaskWorker(10000, "fname", null, WorkerStatus.Available));

            Assert.AreEqual(2, sut.Workers.Count);
            orderer.Verify(o => o.OrderWorkers(It.IsAny<IEnumerable<TaskWorker>>()), Times.Exactly(2));
        }

        [Test]
        public void AddWorker_ShouldNotAddWorkerIfAlreadyExists()
        {
            var workers = new List<TaskWorker>() { SampleTaskWorker };
            var orderer = new Mock<WorkerOrderer>();
            SetupSerialOrdering(orderer);
            var ordererFactory = new Mock<IWorkerOrdererFactory>();
            ordererFactory.Setup(f => f.CreateOrderer(It.IsAny<WorkerOrderingScheme>())).Returns(orderer.Object);
            var sut = new WorkerPool(workers, ordererFactory.Object, WorkerOrderingScheme.AscendingNameScheme);

            sut.AddWorker(SampleTaskWorker);

            Assert.AreEqual(1, sut.Workers.Count);
            orderer.Verify(o => o.OrderWorkers(It.IsAny<IEnumerable<TaskWorker>>()), Times.Exactly(1));
        }

        [Test]
        public void RemoveWorker_ShouldRemoveWorkerIfExists_AndReorderWorkers()
        {
            var workers = new List<TaskWorker>() { SampleTaskWorker };
            var orderer = new Mock<WorkerOrderer>();
            SetupSerialOrdering(orderer);
            var ordererFactory = new Mock<IWorkerOrdererFactory>();
            ordererFactory.Setup(f => f.CreateOrderer(It.IsAny<WorkerOrderingScheme>())).Returns(orderer.Object);
            var sut = new WorkerPool(workers, ordererFactory.Object, WorkerOrderingScheme.AscendingNameScheme);

            sut.RemoveWorker(SampleTaskWorker.Id);

            Assert.AreEqual(0, sut.Workers.Count);
            orderer.Verify(o => o.OrderWorkers(It.IsAny<IEnumerable<TaskWorker>>()), Times.Exactly(2));
        }

        [Test]
        public void RemoveWorker_ShouldNotRemoveWorkerIfNotExists()
        {
            var workers = new List<TaskWorker>() { SampleTaskWorker };
            var orderer = new Mock<WorkerOrderer>();
            SetupSerialOrdering(orderer);
            var ordererFactory = new Mock<IWorkerOrdererFactory>();
            ordererFactory.Setup(f => f.CreateOrderer(It.IsAny<WorkerOrderingScheme>())).Returns(orderer.Object);
            var sut = new WorkerPool(workers, ordererFactory.Object, WorkerOrderingScheme.AscendingNameScheme);

            sut.RemoveWorker(100);

            Assert.AreEqual(1, sut.Workers.Count);
            orderer.Verify(o => o.OrderWorkers(It.IsAny<IEnumerable<TaskWorker>>()), Times.Exactly(1));
        }

        [Test]
        public void Contains_ShouldThrowExceptionIfWorkerIsNull()
        {
            var workers = new List<TaskWorker>() { SampleTaskWorker };
            var orderer = new Mock<WorkerOrderer>();
            SetupSerialOrdering(orderer);
            var ordererFactory = new Mock<IWorkerOrdererFactory>();
            ordererFactory.Setup(f => f.CreateOrderer(It.IsAny<WorkerOrderingScheme>())).Returns(orderer.Object);
            var sut = new WorkerPool(workers, ordererFactory.Object, WorkerOrderingScheme.AscendingNameScheme);

            Assert.Throws<ArgumentNullException>(() => sut.Contains(null));
        }

        [Test]
        public void Contains_ShouldReturnTrueIfWorkerExists()
        {
            var workers = new List<TaskWorker>() { SampleTaskWorker };
            var orderer = new Mock<WorkerOrderer>();
            SetupSerialOrdering(orderer);
            var ordererFactory = new Mock<IWorkerOrdererFactory>();
            ordererFactory.Setup(f => f.CreateOrderer(It.IsAny<WorkerOrderingScheme>())).Returns(orderer.Object);
            var sut = new WorkerPool(workers, ordererFactory.Object, WorkerOrderingScheme.AscendingNameScheme);

            Assert.True(sut.Contains(SampleTaskWorker));
        }

        [Test]
        public void Contains_ShouldReturnFalseIfWorkerNotExists()
        {
            var workers = new List<TaskWorker>() { SampleTaskWorker };
            var orderer = new Mock<WorkerOrderer>();
            SetupSerialOrdering(orderer);
            var ordererFactory = new Mock<IWorkerOrdererFactory>();
            ordererFactory.Setup(f => f.CreateOrderer(It.IsAny<WorkerOrderingScheme>())).Returns(orderer.Object);
            var sut = new WorkerPool(workers, ordererFactory.Object, WorkerOrderingScheme.AscendingNameScheme);

            Assert.False(sut.Contains(new TaskWorker(1235, "NotExist", null, WorkerStatus.Available)));
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
