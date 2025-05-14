using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Sqlite.JsonModels
{
    internal class TableBlockJson : GrammarBlockJson
    {
        public List<List<string>> Rows { get; set; }
    }
}
