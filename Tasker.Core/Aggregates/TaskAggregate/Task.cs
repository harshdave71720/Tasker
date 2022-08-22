using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tasker.Core.Abstractions;
using Tasker.Core.Constants;
using Tasker.Core.Helpers;

namespace Tasker.Core.Aggregates.TaskAggregate
{
    public class Task : Entity<int>
    {
        public string Name { get; private set; }

        public IReadOnlyList<TaskWorker> PossibleWorkers 
        {
            get { return _possibleWorkers; }
        }

        private List<TaskWorker> _possibleWorkers;

        public TaskWorker CurrentWorker { get; private set; }

        public TaskAssignmentStrategy AssignmentStrategy { get; set; }

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
                List<TaskWorker> possibleWorkers, 
                TaskWorker currentWorker = null, 
                TaskAssignmentStrategy taskAssignmetStrategy = TaskAssignmentStrategy.None,
                List<TaskHistoryItem> history = null
            ) 
            : base(id)
        {
            Guard.AgainstEmptyOrWhiteSpace(name);
            Name = name;
            Guard.AgainstNull(possibleWorkers);
            Guard.AgainstEmpty(possibleWorkers);
            _possibleWorkers = possibleWorkers;
            CurrentWorker = currentWorker;
            AssignmentStrategy = taskAssignmetStrategy;
            if (history == null)
                _history = new List<TaskHistoryItem>();
        }

        public Task
            (
            string name, 
            List<TaskWorker> possibleWorkers, 
            TaskWorker currentWorker = null,
            TaskAssignmentStrategy taskAssignmetStrategy = TaskAssignmentStrategy.None,
            List<TaskHistoryItem> history = null
            )
            : this(default(int), name, possibleWorkers, currentWorker, taskAssignmetStrategy, history)
        {
        }

        protected override void IdentityGuards(int id)
        {
            Guard.AgainstNegative(id);
        }

        public void MarkDone()
        {
            if (CurrentWorker == null)
                throw new InvalidOperationException();

            this.AddHistory(TaskCompletionStatus.Done);
            CurrentWorker = null;
        }

        public void Skip()
        {
            if (CurrentWorker == null)
                throw new InvalidOperationException();

            this.AddHistory(TaskCompletionStatus.Skipped);
            CurrentWorker = null;
        }

        private void AddHistory(TaskCompletionStatus taskCompletionStatus)
        {
            this._history.Add(new TaskHistoryItem(DateTime.Now, CurrentWorker.Id, taskCompletionStatus));
        }

        public void AddWorker(TaskWorker worker)
        {
            if(!this._possibleWorkers.Contains(worker))
                this._possibleWorkers.Add(worker);
        }

        public void RemoveWorker(int workerId)
        {
            var workerToRemove = _possibleWorkers.SingleOrDefault(w => w.Id == workerId);
            if (workerToRemove == null)
                return;

            if (workerToRemove.Equals(CurrentWorker))
                throw new InvalidOperationException();

            _possibleWorkers.Remove(workerToRemove);
        }
    }
}
