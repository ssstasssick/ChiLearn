using ChiLearn.Infrastructure;
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

            //_ = await _database.CreateTableAsync<TransactionModel>();
            //_ = await _database.CreateTableAsync<CategoryModel>();
            //_ = await _database.CreateTableAsync<ProfileModel>();
            //_ = await _database.CreateTableAsync<SettingsModel>();
        }

        private string GetDatabasePath(string filename)
        {
            return Path.Combine(_configuration.AppDirectoryPath, filename);
        }
    }
}
