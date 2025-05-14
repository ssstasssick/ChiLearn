using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Sqlite.JsonModels
{
    public class ExampleBlockJson : GrammarBlockJson
    {
        public string Ch { get; set; }
        public string Pn { get; set; }
        public string? Rus { get; set; }
    }
}
