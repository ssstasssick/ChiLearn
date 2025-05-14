using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Core.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services
{
    public class RuleService : IRuleService
    {
        private readonly IRuleRepository _ruleRepository;
        private readonly IGrammarBlockRepository _grammarBlockRepository;
        public RuleService(
            IRuleRepository ruleRepository,
            IGrammarBlockRepository grammarBlockRepository)
        {
            _ruleRepository = ruleRepository;
            _grammarBlockRepository = grammarBlockRepository;
        }
        public async Task<Rule> GetRuleById(int id)
        {
            var rule = await _ruleRepository.GetById(id);
            rule.Content = await _grammarBlockRepository.GetByRuleId(id);

            return rule;
        }

        public Task<List<Rule>> GetRules()
        {
            return _ruleRepository.GetRules();
        }

        public Task<Rule> GetRulesByLevel(int levelId)
        {
            throw new NotImplementedException();
        }
    }
}
