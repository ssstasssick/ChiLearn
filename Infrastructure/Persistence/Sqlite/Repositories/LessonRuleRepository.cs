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
    internal class LessonRuleRepository : ILessonRuleRepository
    {
        private readonly DbConnection _connection;
        private readonly IInfrastructureMapper<LessonRule, LessonRuleModel> _mapper;
        public LessonRuleRepository(
            DbConnection connection,
            IInfrastructureMapper<LessonRule, LessonRuleModel> mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }
        public async Task<LessonRule> AddRuleToLesson(int lessonId, int ruleId)
        {
            return await Create(new LessonRule { LessonId = lessonId, RuleId = ruleId });

        }

        private async Task<LessonRule> Create(LessonRule createdLessonWord)
        {
            await _connection.Init();

            var lessonModel = _mapper.MapToModel(createdLessonWord);

            await _connection.Database.InsertAsync(lessonModel);

            return _mapper.MapToDomain(lessonModel);

        }

        public async Task<int> GetRuleIdByLessonId(int lessonId)
        {
            await _connection.Init();

            var foundLessonId = (await _connection.Database
                .Table<LessonRuleModel>()
                .Where(lr => lr.LessonId.Equals(lessonId))
                .FirstOrDefaultAsync())
                .RuleId;

            return foundLessonId;
                
        }

    }
}
