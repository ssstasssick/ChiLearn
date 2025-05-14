using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entity
{
    public class LessonRule
    {
        public int LessonRuleId { get; set; }
        public int LessonId { get; set; }
        public int RuleId { get; set; }
    }
}
