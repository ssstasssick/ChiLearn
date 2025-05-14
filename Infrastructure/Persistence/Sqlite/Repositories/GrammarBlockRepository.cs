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
    internal class GrammarBlockRepository : IGrammarBlockRepository
    {
        private readonly DbConnection _connection;
        private readonly IInfrastructureMapper<GrammarBlock, GrammarBlockModel> _mapper;
        public GrammarBlockRepository(
            IInfrastructureMapper<GrammarBlock, GrammarBlockModel> mapper,
            DbConnection connection)
        {
            _mapper = mapper;
            _connection = connection;
        }
        public async Task<GrammarBlock> Create(GrammarBlock block)
        {
            await _connection.Init();

            var blockModel = _mapper.MapToModel(block);

            await _connection.Database.
                InsertAsync(blockModel);

            return _mapper.MapToDomain(blockModel);
        }

        public async Task<List<GrammarBlock>> GetByRuleId(int ruleId)
        {
            try
            {
                await _connection.Init();

                var grammarBlocks = await _connection.Database
                    .Table<GrammarBlockModel>()
                    .Where(g => g.RuleId == ruleId)
                    .ToListAsync();

                var grammarBlocksDomain = grammarBlocks.Select(_mapper.MapToDomain).ToList();

                return grammarBlocksDomain;
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
