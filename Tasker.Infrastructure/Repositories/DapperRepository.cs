using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using Tasker.Infrastructure.Settings;
using MySql.Data.MySqlClient;

namespace Tasker.Infrastructure.Repositories
{
    abstract internal class DapperRepository
    {
        private readonly DataStoreSettings _settings;

        public DapperRepository(IOptions<DataStoreSettings> settingOptions)
        {
            _settings = settingOptions.Value;
        }

        protected IDbConnection GetDbConnection => new MySqlConnection(_settings.TaskerStore);
    }
}
