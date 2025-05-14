using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Sqlite.JsonModels
{
    internal class TextBlockJson : GrammarBlockJson
    {
        public string Text { get; set; }
    }
}
