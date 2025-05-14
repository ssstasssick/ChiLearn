using Core.Domain.Entity;
using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Sqlite.Mappers
{
    internal class LessonRuleMapper : IInfrastructureMapper<LessonRule, LessonRuleModel>
    {
        public LessonRule MapToDomain(LessonRuleModel infrastucture)
        {
            return new LessonRule
            {
                LessonId = infrastucture.LessonId,
                LessonRuleId = infrastucture.LessonRuleId,
                RuleId = infrastucture.RuleId
            };
        }

        public LessonRuleModel MapToModel(LessonRule domain)
        {
            return new LessonRuleModel
            {
                LessonRuleId = domain.LessonRuleId,
                LessonId = domain.LessonId,
                RuleId = domain.RuleId
            };
        }
    }
}
