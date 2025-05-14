using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Sqlite.Models
{
    public class GrammarBlockModel
    {
        [PrimaryKey, AutoIncrement]
        public int GrammarBlockId { get; set; }
        public int RuleId { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Ch { get; set; }
        public string Pn { get; set; }
        public string Rus { get; set; }
    }
}
