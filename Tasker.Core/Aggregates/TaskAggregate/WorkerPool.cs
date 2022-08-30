using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Tasker.Core.Helpers;
using Tasker.Core.TaskWorkerOrdering;
using Tasker.Core.TaskWorkerOrdering.Factories;
using Tasker.Core.Constants;

namespace Tasker.Core.Aggregates.TaskAggregate
{
    public class WorkerPool
    {
        private Dictionary<int, TaskWorker> _workers;
        private IWorkerOrdererFactory _ordererFactory;
        private WorkerOrderingScheme _orderingScheme;

        private int _poolSize => _workers.Count;
        public IReadOnlyList<TaskWorker> Workers 
        {
            get
            {
                return _workers.Values.ToList().AsReadOnly();
            }
        }

        public WorkerPool(List<TaskWorker> workers, IWorkerOrdererFactory ordererFactory, WorkerOrderingScheme orderingScheme)
        {
            Guard.AgainstNull(workers);
            Guard.AgainstEmpty(workers);
            Guard.AgainstNull(ordererFactory);

            _ordererFactory = ordererFactory;
            _orderingScheme = orderingScheme;
            OrderWorkers(workers);
        }

        public TaskWorker GetNextWorker(TaskWorker currentWorker)
        {
            Guard.AgainstNull(currentWorker);
            var currentWorkerOrder = _workers.Values.Single(x => x.Equals(currentWorker)).Order;
            var nextOrder = currentWorkerOrder + 1;
            if (nextOrder > _poolSize)
                nextOrder = 1;
            while (nextOrder != currentWorkerOrder && _workers[nextOrder].Status == WorkerStatus.Absent)
            {
                nextOrder = nextOrder + 1 <= _poolSize ? nextOrder + 1 : 1;
            }

            var nextWorker = _workers[nextOrder];
            return nextWorker.Status == WorkerStatus.Available ? nextWorker : null;
        }

        private void OrderWorkers(IEnumerable<TaskWorker> workers)
        {
            _ordererFactory.CreateOrderer(_orderingScheme).OrderWorkers(workers);
            _workers = workers.ToDictionary(w => w.Order);
        }

        public void AddWorker(TaskWorker taskWorker)
        {
            Guard.AgainstNull(taskWorker);

            var workers = _workers.Values.ToList();
            if (workers.Contains(taskWorker))
                return;

            workers.Add(taskWorker);
            OrderWorkers(workers);
        }

        public void RemoveWorker(int workerId)
        {
            var workers = _workers.Values.ToList();
            var worker = workers.SingleOrDefault(w => w.Id == workerId);

            if (worker == null)
                return;
            workers.Remove(worker);
            OrderWorkers(workers);
        }

        public bool Contains(TaskWorker worker)
        {
            Guard.AgainstNull(worker);
            return _workers.Values.Contains(worker);
        }
    }
}
