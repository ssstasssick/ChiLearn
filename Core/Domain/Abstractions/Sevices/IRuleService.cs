using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Abstractions.Sevices
{
    public interface IRuleService
    {
        Task<Rule> GetRuleByLevel(int lessonId);
        Task<Rule> GetRuleById(int id);
        Task<List<Rule>> GetRules();
        Task<List<Rule>> GetLearnedRules();
    }
}
