using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Sqlite.Configuration
{
    public class Constants
    {
        public const string DatabaseFileName = "ChiLearnLocalDb.db3";
        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;
        public static readonly Dictionary<int, string> HskCsvFileName = new()
        {
            [1] = "Hsk-1-Csv.csv",
            //[2] = "Hsk-2-CSV",
            //[3] = "Hsk-3-CSV",

        };
    }
}
