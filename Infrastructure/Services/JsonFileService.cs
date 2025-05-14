using ChiLearn.Infrastructure;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Configuration;
using Infrastructure.Persistence.Sqlite.JsonModels;
using Infrastructure.Persistence.Sqlite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    internal class JsonFileService
    {
        private readonly IInfrastructureMapper<Rule, RuleModel> _ruleMapper;
        private readonly IInfrastructureMapper<GrammarBlock, GrammarBlockModel> _grammarBlockMapper;
        private readonly InfrastuctureConfiguration _configuration;
        public JsonFileService(
            InfrastuctureConfiguration configuration,
            IInfrastructureMapper<GrammarBlock, GrammarBlockModel> grammarBlockMapper,
            IInfrastructureMapper<Rule, RuleModel> ruleMapper
            )
        {
            _ruleMapper = ruleMapper;
            _grammarBlockMapper = grammarBlockMapper;
            _configuration = configuration;
        }

        public async Task<List<RuleModel>> GetRulesFromJsonAsync()
        {
            var jsonFilePath = Path.Combine(_configuration.AppDirectoryPath, "Resources", "Raw", Constants.RuleJsonFileName);
            if (!File.Exists(jsonFilePath))
                return new List<RuleModel>();

            var jsonContent = await File.ReadAllTextAsync(jsonFilePath);

            var options = new JsonSerializerOptions
            {
                Converters = { new GrammarBlockConverter() },
                PropertyNameCaseInsensitive = true
            };

            var rules = JsonSerializer.Deserialize<List<Rule>>(jsonContent, options);
            if (rules == null || rules.Count == 0)
                return new List<RuleModel>();

            var ruleModels = new List<RuleModel>();
            foreach (var rule in rules)
            {
                var ruleModel = _ruleMapper.MapToModel(rule);
                ruleModels.Add(ruleModel);

                foreach (var grammarBlock in rule.Content)
                {
                    var grammarBlockModel = _grammarBlockMapper.MapToModel(grammarBlock);
                    grammarBlockModel.RuleId = ruleModel.RuleId;
                    ruleModel.GrammarBlocks.Add(grammarBlockModel);
                }
            }

            return ruleModels;
        }
    }
}
