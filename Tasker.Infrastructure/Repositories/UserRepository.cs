using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tasker.Application.Repositories;
using Tasker.Core.Aggregates.UserAggregate;
using Tasker.Infrastructure.Settings;
using Dapper;
using Tasker.Core.Constants;
using System.Linq;

namespace Tasker.Infrastructure.Repositories
{
    internal class UserRepository : DapperRepository, IUserRepository
    {
        public UserRepository(IOptions<DataStoreSettings> settingOptions) : base(settingOptions) { }

        public async Task<bool> Delete(int id)
        {
            using (var connection = GetDbConnection)
            {
                var sql = @"UPDATE USER SET STATUS = @status WHERE Id = @id;";

                var user = await connection.ExecuteAsync(sql, new { id, status = UserStatus.Inactive });
                return true;
            }
        }

        public async Task<User> Get(int id)
        {
            using (var connection = GetDbConnection)
            {
                var sql = @"SELECT Id, FirstName, LastName, Email, Worker_Status as WorkerStatus 
                            FROM USER 
                            WHERE ID = @id AND Status = @status;";

                var user = await connection.QuerySingleOrDefaultAsync(sql, new { id, status = UserStatus.Active });
                return user != null
                    ? new User
                    (
                        Convert.ChangeType(user.Id, typeof(int)),
                        Convert.ChangeType(user.Email, typeof(string)),
                        Convert.ChangeType(user.FirstName, typeof(string)),
                        Convert.ChangeType(user.LastName, typeof(string)),
                        (WorkerStatus)user.WorkerStatus
                    )
                    : null;
            }
        }

        public async Task<User> Get(string email)
        {
            using (var connection = GetDbConnection)
            {
                var sql = @"SELECT Id, FirstName, LastName, Email, Worker_Status as WorkerStatus 
                            FROM USER 
                            WHERE Email = @email AND Status = @status;";

                var user = await connection.QuerySingleOrDefaultAsync(sql, new { email, status = UserStatus.Active });
                return user != null
                    ? new User
                    (
                        Convert.ChangeType(user.Id, typeof(int)),
                        Convert.ChangeType(user.Email, typeof(string)),
                        Convert.ChangeType(user.FirstName, typeof(string)),
                        Convert.ChangeType(user.LastName, typeof(string)),
                        (WorkerStatus)user.WorkerStatus
                    )
                    : null;
            }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            using (var connection = GetDbConnection)
            {
                var sql = @"SELECT Id, FirstName, LastName, Email, Worker_Status as WorkerStatus 
                            FROM USER WHERE STATUS = @status;";

                var user = await connection.QueryAsync(sql, new { status = UserStatus.Active });
                return user.Select(u => new User
                    (
                        Convert.ChangeType(u.Id, typeof(int)),
                        Convert.ChangeType(u.Email, typeof(string)),
                        Convert.ChangeType(u.FirstName, typeof(string)),
                        Convert.ChangeType(u.LastName, typeof(string)),
                        (WorkerStatus)u.WorkerStatus
                    )).ToList();
            }
        }

        public async Task<IEnumerable<User>> GetAll(List<int> ids)
        {
            using (var connection = GetDbConnection)
            {
                var sql = @"SELECT Id, FirstName, LastName, Email, Worker_Status as WorkerStatus 
                            FROM USER WHERE Id in @ids";

                var user = await connection.QueryAsync(sql, new { ids });
                return user.Select(u => new User
                    (
                        Convert.ChangeType(u.Id, typeof(int)),
                        Convert.ChangeType(u.Email, typeof(string)),
                        Convert.ChangeType(u.FirstName, typeof(string)),
                        Convert.ChangeType(u.LastName, typeof(string)),
                        (WorkerStatus)u.WorkerStatus
                    )).ToList();
            }
        }

        public async Task<User> Save(User item)
        {
            if (item.Id == default(int))
                return await Insert(item);
            else
                return await Update(item);
        }

        private async Task<User> Insert(User item)
        {
            using (var connection = GetDbConnection)
            {
                var sql = @"INSERT INTO USER(
	                            FIRSTNAME,LASTNAME,EMAIL,STATUS,WORKER_STATUS
                            )
                            VALUES(
		                            @FirstName, @LastName, @EmailAddress, @Status, @WorkerStatus
                            );
                            SELECT LAST_INSERT_ID();";

                item.Id = await connection.QuerySingleAsync<int>(sql, item);
                return item;
            }
        }

        private async Task<User> Update(User item)
        {
            using (var connection = GetDbConnection)
            {
                var sql = @"UPDATE USER 
                            SET
                                FIRSTNAME = @FirstName,
	                            LASTNAME = @LastName,
                                WORKER_STATUS= @WorkerStatus
                            WHERE ID = @Id;";

                await connection.ExecuteAsync(sql, item);
                return item;
            }
        }
    }
}
