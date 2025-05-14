using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Sqlite.JsonModels
{
    public class RuleJson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public List<GrammarBlockJson> Content { get; set; }
    }
}
