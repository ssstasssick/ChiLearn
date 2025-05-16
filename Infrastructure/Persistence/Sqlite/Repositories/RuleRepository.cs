using Core.Domain.Entity;
using Core.Persistence;
using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Configuration;
using Infrastructure.Persistence.Sqlite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Sqlite.Repositories
{
    internal class RuleRepository : IRuleRepository
    {
        private readonly DbConnection _connection;
        private readonly IInfrastructureMapper<Rule, RuleModel> _mapper;
        public RuleRepository(
            DbConnection connection,
            IInfrastructureMapper<Rule, RuleModel> mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }
        public async Task<bool> AnyAsync()
        {
            await _connection.Init();

            var exists = await _connection.Database
                .Table<RuleModel>()
                .Take(1)
                .CountAsync();

            return exists > 0;
        }

        public async Task<Rule> Create(Rule rule)
        {
            await _connection.Init();
            var ruleModel = _mapper.MapToModel(rule);
            await _connection.Database
                .InsertAsync(ruleModel);
            return _mapper.MapToDomain(ruleModel);
        }

        public async Task<Rule?> GetById(int ruleId)
        {
            await _connection.Init();

            var rule = await _connection.Database
                .Table<RuleModel>()
                .FirstOrDefaultAsync(r => r.RuleId.Equals(ruleId));

            return rule is not null
                ? _mapper.MapToDomain(rule)
                : null;
        }

        public async Task<List<Rule>> GetRules()
        {
            await _connection.Init();

            var rules = await _connection.Database.
                Table<RuleModel>().
                ToListAsync();

            var domeinLessons = rules.
                Select(_mapper.MapToDomain).
                ToList();

            return domeinLessons;
        }


    }
}
