using Core.Domain.Abstractions.Sevices;
using Infrastructure.Persistence.Sqlite.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    internal class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly DbConnection _dbConnection;
        private readonly IDataBaseSeeder _databaseSeeder;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(
            DbConnection dbConnection,
            IDataBaseSeeder databaseSeeder, 
            ILogger<DatabaseInitializer> logger)
        {
            _dbConnection = dbConnection;
            _databaseSeeder = databaseSeeder;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _dbConnection.Init();
                await _databaseSeeder.SeedDatabase();
                _logger.LogInformation("Database initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database initialization failed");
                throw;
            }

        }
    }
}
