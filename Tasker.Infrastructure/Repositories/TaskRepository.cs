using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tasker.Core.Aggregates;
using TaskAggregate = Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Application.Repositories;
using System.Transactions;
using Dapper;
using Microsoft.Extensions.Options;
using Tasker.Infrastructure.Settings;

namespace Tasker.Infrastructure.Repositories
{
    public class TaskRepository : DapperRepository, ITaskRepository
    {
        public TaskRepository(IOptions<DataStoreSettings> settingOptions) : base(settingOptions) {}

        public async Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<TaskAggregate.Task> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TaskAggregate.Task>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<TaskAggregate.Task> Save(TaskAggregate.Task item)
        {
            if (item.Id == default(int))
                return await Insert(item);
            else
                return await Update(item);
        }

        private async Task<TaskAggregate.Task> Update(TaskAggregate.Task item)
        {
            throw new NotImplementedException();
        }

        private async Task<TaskAggregate.Task> Insert(TaskAggregate.Task item)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var taskId = 0;
                using (var connection = GetDbConnection)
                {
                    var sql = @"INSERT INTO Task(Name, Created_On, Currentworker_Id, Worker_Ordering_Scheme)
                            VALUES (@name, @createdon, @currentWorkerId, @workerOrderingScheme);
                            SELECT LAST_INSERT_ID();
                            ";
                    taskId = await connection.QuerySingleAsync<int>(
                        sql,
                        new
                        {
                            name = item.Name,
                            createdOn = item.CreatedOn,
                            currentWorkerId = item.CurrentWorker.Id,
                            workerOrderingScheme = item.WorkerOrderingScheme
                        }
                    );
                    foreach (var worker in item.PossibleWorkers)
                    {
                        sql = @"INSERT INTO Task_Worker(TaskId, WorkerId)
                                VALUES (@taskId, @workerId)";
                        await connection.ExecuteAsync(sql, new { taskId = taskId, workerId = worker.Id });
                    }
                }
                scope.Complete();
                item.Id = taskId;
            }

            return item;
        }
    }
}
