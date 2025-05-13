using ChiLearn.Infrastructure;
using Core.Domain.Abstractions.Sevices;
using Infrastructure.Persistence.Sqlite.Models;
using SQLite;

namespace Infrastructure.Persistence.Sqlite.Configuration
{
    internal class DbConnection
    {
        private SQLiteAsyncConnection? _database;
        private readonly InfrastuctureConfiguration _configuration;
        public DbConnection(InfrastuctureConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public SQLiteAsyncConnection Database
        {
            get
            {
                if (_database is null)
                {
                    throw new NullReferenceException("Database connection is null. Need to call Init() before using DbConnection");
                }

                return _database;
            }
        }

        public async ValueTask Init()
        {
            if (_database is not null)
            {
                //if (File.Exists(GetDatabasePath(Constants.DatabaseFileName)))
                //{
                //    File.Delete(GetDatabasePath(Constants.DatabaseFileName));
                //}
                return;
            }

            _database = new SQLiteAsyncConnection(GetDatabasePath(Constants.DatabaseFileName), Constants.Flags);
            await CreateTables();

        }

        private async Task CreateTables()
        {
            if (_database is null)
            {
                throw new NullReferenceException("Local DB connection is not initialized");
            }

            _ = await _database.CreateTableAsync<LessonModel>();
            _ = await _database.CreateTableAsync<WordModel>();
            _ = await _database.CreateTableAsync<LessonWordModel>();
            _ = await _database.CreateTableAsync<RuleModel>();
            _ = await _database.CreateTableAsync<LessonRuleModel>();
        }

        private string GetDatabasePath(string filename)
        {
            return Path.Combine(_configuration.AppDirectoryPath, filename);
        }
    }
}
