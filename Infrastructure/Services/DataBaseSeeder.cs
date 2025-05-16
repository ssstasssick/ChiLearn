using ChiLearn.Infrastructure;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Core.Persistence;
using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Configuration;
using Infrastructure.Persistence.Sqlite.Models;
using Infrastructure.Persistence.Sqlite.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    internal class DataBaseSeeder : IDataBaseSeeder
    {
        private readonly IWordRepository _wordRepository;
        private readonly ILessonWordRepository _lessonWordRepository;
        private readonly ILessonRuleRepository _lessonRuleRepository;
        private readonly ICsvDataService _csvReaderService;
        private readonly ILessonRepository _lessonRepository;
        private readonly IRuleRepository _ruleRepository;
        private readonly IGrammarBlockRepository _grammarBlockRepository;
        private readonly JsonFileService _jsonFileService;

        private readonly IInfrastructureMapper<Rule, RuleModel> _ruleMapper;
        private readonly IInfrastructureMapper<GrammarBlock, GrammarBlockModel> _grammarBlockMapper;

        public DataBaseSeeder(
        IWordRepository wordRepository,
        ILessonRepository lessonRepository,
        IRuleRepository ruleRepository,
        IGrammarBlockRepository grammarBlockRepository,
        ILessonWordRepository lessonWordRepository,
        ILessonRuleRepository lessonRuleRepository,
        ICsvDataService csvReaderService,
        JsonFileService jsonFileService,
        IInfrastructureMapper<Rule, RuleModel> ruleMapper,
        IInfrastructureMapper<GrammarBlock, GrammarBlockModel> grammarBlockMapper)
        {
            _wordRepository = wordRepository;
            _lessonRepository = lessonRepository;
            _ruleRepository = ruleRepository;
            _grammarBlockRepository = grammarBlockRepository;

            _lessonWordRepository = lessonWordRepository;
            _lessonRuleRepository = lessonRuleRepository;

            _csvReaderService = csvReaderService;
            _jsonFileService = jsonFileService;

            _ruleMapper = ruleMapper;
            _grammarBlockMapper = grammarBlockMapper;
        }

        public async Task SeedDatabase()
        {
            for (int level = 1; level <= Constants.HskCsvFileName.Count; level++)
            {
                await SeedWords(level);
                await SeedLessons(10, level);
            }
            await SeedGrammar();
        }

        private async Task SeedWords(int hskLevel)
        {
            if (await _wordRepository.AnyAsync(hskLevel) is false)
            {
                var words = _csvReaderService.GetWordsFromCsv(hskLevel);

                if (words.Count.Equals(0))
                {
                    throw new InvalidOperationException($"No words found in CSV for HSK level {hskLevel}");
                }

                words.ForEach(w =>
                {
                    w.HskLevel = hskLevel;
                    var audioFileName = $"{ConverterChiWordToUnicode.ConvertToUnicode(w.ChiWord)}";
                    w.AudioPath = audioFileName;
                });

                foreach (var word in words)
                {
                    await _wordRepository.Create(word);
                }
            }
        }

        private async Task SeedLessons(int wordsInLesson, int hskLevel)
        {
            if (await _lessonRepository.AnyAsync(hskLevel) is false)
            {
                var words = await _wordRepository.GetWordsByHskLevel(hskLevel);
                var allLessonCount = (await _lessonRepository.GetAll()).Count;
                var wordIds = words.Select(w => w.WordId).ToList();

                for (int i = 0; i < wordIds.Count; i += wordsInLesson)
                {
                    var lesson = new Lesson
                    {
                        HskLevel = hskLevel,
                        LessonNum = i / wordsInLesson + allLessonCount + 1,
                        Description = $"Слова {i + 1}-{Math.Min(i + wordsInLesson, wordIds.Count)} уровня HSK {hskLevel}"
                    };
                    lesson = await _lessonRepository.Create(lesson);
                    await _lessonWordRepository.AddWordsToLesson(lesson.LessonId, wordIds.Skip(i).Take(wordsInLesson));
                }
            }
        }

        private async Task SeedGrammar()
        {
            try
            {
                if (await _ruleRepository.AnyAsync())
                    return;

                var rules = await _jsonFileService.GetRulesFromJsonAsync();
                if (rules == null || rules.Count == 0)
                    return;

                var savedRules = new List<Rule>();

                foreach (var ruleModel in rules)
                {
                    var domainRule = _ruleMapper.MapToDomain(ruleModel);
                    var createdRule = await _ruleRepository.Create(domainRule);

                    savedRules.Add(createdRule);

                    foreach (var block in ruleModel.GrammarBlocks)
                    {
                        var blockDomain = _grammarBlockMapper.MapToDomain(block);
                        blockDomain.RuleId = createdRule.Id;
                        await _grammarBlockRepository.Create(blockDomain);
                    }
                }

                var lessons = await _lessonRepository.GetAll();
                if (lessons == null || lessons.Count == 0)
                    return;

                int ruleCount = savedRules.Count;
                for (int i = 0; i < lessons.Count; i++)
                {
                    var lesson = lessons[i];
                    var rule = savedRules[i % ruleCount];
                    await _lessonRuleRepository.AddRuleToLesson(lesson.LessonId, rule.Id);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
