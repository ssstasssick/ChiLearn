using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Sqlite.Models
{
    internal class LessonRuleModel
    {
        [PrimaryKey, AutoIncrement]
        public int LessonRuleId { get; set; }
        [Indexed(Name = "IX_LessonRule", Unique = true, Order = 2)]
        public int RuleId { get; set; }
        [Indexed(Name = "IX_LessonRule", Unique = true, Order = 1)]
        public int LessonId { get; set; }
    }
}
