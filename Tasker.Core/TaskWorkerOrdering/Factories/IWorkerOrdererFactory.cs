using Tasker.Core.Constants;

namespace Tasker.Core.TaskWorkerOrdering.Factories
{
    public interface IWorkerOrdererFactory
    {
        public WorkerOrderer CreateOrderer(WorkerOrderingScheme assignmentStrategy);
    }
}
