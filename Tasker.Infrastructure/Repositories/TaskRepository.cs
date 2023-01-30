using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaskAggregate = Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Application.Repositories;
using System.Transactions;
using Dapper;
using Microsoft.Extensions.Options;
using Tasker.Infrastructure.Settings;
using System.Linq;
using Tasker.Core.Helpers;
using Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Core.Aggregates.UserAggregate;

namespace Tasker.Infrastructure.Repositories
{
    public class TaskRepository : DapperRepository, ITaskRepository
    {
        private readonly CoreObjectConvertor _coreObjectConvertor;
        public TaskRepository(IOptions<DataStoreSettings> settingOptions, CoreObjectConvertor coreObjectConvertor) : base(settingOptions) 
        {
            _coreObjectConvertor = coreObjectConvertor;
        }

        public async Task<bool> Delete(int id)
        {
            if (!await Exists(id))
                return false;

            using (var scope = new TransactionScope())
            {
                using (var connection = GetDbConnection)
                {
                    var sql = @"DELETE FROM TASKWORKER WHERE TASKID = @id";
                    await connection.ExecuteAsync(sql, new { id = id });
                    sql = @"DELETE FROM TASKHISTORY WHERE TASKID = @id";
                    await connection.ExecuteAsync(sql, new { id = id });
                    sql = @"DELETE FROM TASK WHERE ID = @id";
                    await connection.ExecuteAsync(sql, new { id = id });
                }

                scope.Complete();
            }

            return true;
        }

        public async Task<TaskAggregate.Task> Get(int id)
        {
            if (! await Exists(id))
                return null;
            using (var connection = GetDbConnection)
            {
                var sql = @"SELECT U.Id, U.FirstName, U.LastName, U.WorkerStatus
                            FROM TASKWORKER TW 
                            INNER JOIN USER U
                            ON TW.WORKERID = U.ID
                            WHERE TW.TASKID = @taskId
                            ";
                var workers = (await connection.QueryAsync(sql: sql,param: new { taskId = id }))
                                .Select(tw => _coreObjectConvertor.ToTaskWorker(tw)).Cast<TaskWorker>().ToList();
                sql = @"SELECT CREATEDON AS TimeStamp, WorkerId, CompletionStatus as Status 
                        FROM TASKHISTORY WHERE TASKID = @taskId
                        ";

                var history = (await connection.QueryAsync(sql, new { taskId = id }))
                                .Select(th => _coreObjectConvertor.ToTaskHistoryItem(th)).Cast<TaskHistoryItem>().ToList();
                sql = @"SELECT Id, Name, CreatedOn, CurrentWorkerId, WorkerOrderingScheme 
                        FROM TASK WHERE ID = @id;
                        ";

                var task = await connection.QuerySingleAsync(sql, new { id });
                TaskWorker currentWorker = null;
                if (task.CurrentWorkerId != null)
                    currentWorker = workers.Single(w => w.Id == task.CurrentWorkerId);
                return _coreObjectConvertor.ToTask(task, workers, currentWorker, history);
            }
        }

        public async Task<IEnumerable<TaskAggregate.Task>> GetAll(User user)
        {
            using (var connection = GetDbConnection)
            {
                var sql = @"SELECT T.Id AS kId, T.Name, CreatedOn, CurrentWorkerId, WorkerOrderingScheme,
                            U.Id as WorkerId, U.FirstName, U.LastName, U.WorkerStatus
                            FROM TASK T
                            INNER JOIN TASKWORKER TW ON 
                            T.ID = TW.TASKID AND TW.WORKERID = @UserId
                            INNER JOIN USER U
                            ON T.CURRENTWORKERID = U.ID
                            ";

                var tasks = await connection.QueryAsync(sql, new { UserId = user.Id });
                var tasksToReturn = new List<TaskAggregate.Task>();
                foreach (var task in tasks)
                {
                    TaskWorker worker = null;
                    List <TaskWorker> workers = new List<TaskWorker>();
                    if (task.CurrentWorkerId != null)
                    {
                        worker = _coreObjectConvertor.ToTaskWorker(new { Id = task.WorkerId, task.FirstName, task.LastName, task.WorkerStatus });
                        workers.Add(worker);
                    }
                    tasksToReturn.Add(_coreObjectConvertor.ToTask(task, workers, worker, new List<TaskHistoryItem>()));
                }
                return tasksToReturn;
            }
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
            using (TransactionScope scope = new TransactionScope())
            {
                using (var connection = GetDbConnection)
                {
                    var sql = @"UPDATE TASK SET
                                NAME = @name,
                                CURRENTWORKERID = @currentWorkerId,
                                WORKERORDERINGSCHEME = @workerOrderingScheme
                                WHERE ID = @id
                    ";

                    await connection.ExecuteAsync(sql, new 
                    { 
                        id = item.Id,
                        name = item.Name,
                        currentWorkerId = item.CurrentWorker.Id,
                        workerOrderingScheme = item.WorkerOrderingScheme
                    });

                    IEnumerable<TaskHistoryItem> historyItemsToInsert = null;
                    sql = @"SELECT MAX(CREATEDON) FROM TASKHISTORY WHERE TASKID = @taskId";
                    var lastTimeStamp = await connection.ExecuteScalarAsync<DateTime?>(sql, new { taskID = item.Id });
                    historyItemsToInsert = lastTimeStamp == null ? item.History : item.History.Where(h => h.TimeStamp > lastTimeStamp);

                    foreach (var historyItem in historyItemsToInsert)
                    {
                        sql = @"INSERT INTO TASKHISTORY(TASKID, CREATEDON, WORKERID, COMPLETIONSTATUS) 
                                VALUES(@taskId, @createdOn, @workerId, @completionStatus)";

                        await connection.ExecuteAsync(sql, new 
                        { 
                            taskId = item.Id,
                            createdOn = DateTime.Now,
                            workerID = historyItem.WorkerId,
                            completionStatus = historyItem.Status
                        });
                    }

                    scope.Complete();
                }
            }

            return item;
        }

        private async Task<TaskAggregate.Task> Insert(TaskAggregate.Task item)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var taskId = 0;
                using (var connection = GetDbConnection)
                {
                    var sql = @"INSERT INTO Task(Name, CreatedOn, CurrentworkerId, WorkerOrderingScheme)
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
                        sql = @"INSERT INTO TaskWorker(TaskId, WorkerId)
                                VALUES (@taskId, @workerId)";
                        await connection.ExecuteAsync(sql, new { taskId = taskId, workerId = worker.Id });
                    }
                }
                scope.Complete();
                item.Id = taskId;
            }

            return item;
        }

        public async Task<bool> Exists(int id)
        {
            using (var connection = GetDbConnection)
            {
                var sql = @"SELECT 1 FROM TASK WHERE ID = @id";
                var found = await connection.ExecuteScalarAsync<int>(sql, new { id = id });
                return found == 1;
            }
        }

        public async Task<IEnumerable<TaskAggregate.Task>> GetAll()
        {
            using (var connection = GetDbConnection)
            {
                var sql = @"SELECT T.Id AS Id, T.Name, CreatedOn, CurrentWorkerId, WorkerOrderingScheme,
                            U.Id as WorkerId, U.FirstName, U.LastName, U.WorkerStatus
                            FROM TASK T
                            INNER JOIN USER U
                            ON T.CURRENTWORKERID = U.ID
                            ";

                var tasks = await connection.QueryAsync(sql);
                var tasksToReturn = new List<TaskAggregate.Task>();
                foreach (var task in tasks)
                {
                    TaskWorker worker = null;
                    List<TaskWorker> workers = new List<TaskWorker>();
                    if (task.CurrentWorkerId != null)
                    {
                        worker = _coreObjectConvertor.ToTaskWorker(new { Id = task.WorkerId, task.FirstName, task.LastName, task.WorkerStatus });
                        workers.Add(worker);
                    }
                    tasksToReturn.Add(_coreObjectConvertor.ToTask(task, workers, worker, new List<TaskHistoryItem>()));
                }
                return tasksToReturn;
            }
        }
    }
}
