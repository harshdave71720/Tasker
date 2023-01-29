using System;
using Tasker.Core.Constants;

namespace Tasker.Core.TaskWorkerOrdering.Factories
{
    public class WorkerOrdererFactory : IWorkerOrdererFactory
    {
        public WorkerOrderer CreateOrderer(WorkerOrderingScheme orderingScheme)
        {
            switch (orderingScheme)
            {
                case WorkerOrderingScheme.None:
                    return new AscendingNameWorkerOrderer();
                case WorkerOrderingScheme.AscendingNameScheme:
                    return new AscendingNameWorkerOrderer();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
