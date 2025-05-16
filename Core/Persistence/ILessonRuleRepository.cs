using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistence
{
    public interface ILessonRuleRepository
    {
        Task<LessonRule> AddRuleToLesson(int lessonId, int ruleId);
        Task<int> GetRuleIdByLessonId(int lessonId);
    }
}
