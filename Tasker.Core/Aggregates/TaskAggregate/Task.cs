using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tasker.Core.Abstractions;
using Tasker.Core.Constants;
using Tasker.Core.Helpers;
using Tasker.Core.TaskWorkerOrdering;

namespace Tasker.Core.Aggregates.TaskAggregate
{
    public class Task : Entity<int>
    {
        public string Name { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public IReadOnlyList<TaskWorker> PossibleWorkers 
        {
            get { return _workerPool.Workers; }
        }

        private WorkerPool _workerPool;

        private TaskWorker _currentWorker;

        public TaskWorker CurrentWorker 
        {
            get { return _currentWorker; }
            set 
            {
                if (!_workerPool.Contains(value))
                    throw new InvalidOperationException();
                _currentWorker = value;
            }
        }

        //public WorkerOrderingScheme WorkerOrderingScheme { get; private set; }

        private List<TaskHistoryItem> _history;

        public IReadOnlyList<TaskHistoryItem> History 
        { 
            get 
            { 
                return _history.AsReadOnly();
            } 
        }

        public Task
        (
            int id, 
            string name,
            WorkerPool workerPool, 
            TaskWorker currentWorker = null, 
            WorkerOrderingScheme workerOrderingScheme = WorkerOrderingScheme.AscendingNameScheme,
            List<TaskHistoryItem> history = null,
            DateTime? createdOn = null
        ) 
        : base(id)
        {
            Guard.AgainstEmptyOrWhiteSpace(name);
            Name = name;
            Guard.AgainstNull(workerPool);
            _workerPool = workerPool;
            if (currentWorker?.Status == WorkerStatus.Absent)
                throw new InvalidOperationException();

            if(currentWorker != null && !workerPool.Contains(currentWorker))
                throw new InvalidOperationException();

            _currentWorker = currentWorker;
            //WorkerOrderingScheme = workerOrderingScheme;
            if (history == null)
                _history = new List<TaskHistoryItem>();

            CreatedOn = createdOn ?? DateTime.Now.Date;
        }

        public Task
        (
        string name,
        WorkerPool workerPool,
        TaskWorker currentWorker = null,
        WorkerOrderingScheme workerOrderingScheme = WorkerOrderingScheme.AscendingNameScheme,
        List<TaskHistoryItem> history = null,
        DateTime? createdOn = null
        )
        : this(default(int), name, workerPool, currentWorker, workerOrderingScheme, history, createdOn){}

        protected override void IdentityGuards(int id)
        {
            Guard.AgainstNegative(id);
        }

        public void MarkDone()
        {
            if (_currentWorker == null)
                throw new InvalidOperationException();

            this.AddHistory(TaskCompletionStatus.Done);
            _currentWorker = _workerPool.GetNextWorker(_currentWorker);
        }

        public void Skip()
        {
            if (_currentWorker == null)
                throw new InvalidOperationException();

            this.AddHistory(TaskCompletionStatus.Skipped);
            _currentWorker = _workerPool.GetNextWorker(_currentWorker);
        }

        private void AddHistory(TaskCompletionStatus taskCompletionStatus)
        {
            this._history.Add(new TaskHistoryItem(DateTime.Now, _currentWorker.Id, taskCompletionStatus));
        }

        public void AddWorker(TaskWorker worker)
        {
            Guard.AgainstNull(worker);
            _workerPool.AddWorker(worker);
        }

        public void RemoveWorker(int workerId)
        {
            if (workerId == _currentWorker?.Id)
                throw new InvalidOperationException();
            _workerPool.RemoveWorker(workerId);
        }
    }
}
