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
        private readonly ILessonRuleRepository _lessonRuleRepository;
        private readonly ILessonRepository _lessonRepository;
        public RuleService(
            IRuleRepository ruleRepository,
            IGrammarBlockRepository grammarBlockRepository,
            ILessonRuleRepository lessonRuleRepository,
            ILessonRepository lessonRepository)
        {
            _ruleRepository = ruleRepository;
            _grammarBlockRepository = grammarBlockRepository;
            _lessonRuleRepository = lessonRuleRepository;
            _lessonRepository = lessonRepository;
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

        public async Task<Rule> GetRuleByLevel(int lessonId)
        {
            var rule = await _ruleRepository.GetById(await _lessonRuleRepository.GetRuleIdByLessonId(lessonId));
            rule.Content = await _grammarBlockRepository.GetByRuleId(rule.Id);

            return rule;
        }

        public async Task<List<Rule>> GetLearnedRules()
        {
            var completedLessonsIds = (await _lessonRepository.GetAll())
                .Where(l => l.CompletedPractice == true)
                .Select(l => l.LessonId)
                .ToList();

            var learnedRules = new List<Rule>();

            foreach (var lessonId in completedLessonsIds)
            {
                var ruleId = await _lessonRuleRepository.GetRuleIdByLessonId(lessonId);
                var rule = await _ruleRepository.GetById(ruleId);
                rule.Content = await _grammarBlockRepository.GetByRuleId(rule.Id);
                if (rule != null)
                    learnedRules.Add(rule);
            }

            return learnedRules;
        }
    }
}
