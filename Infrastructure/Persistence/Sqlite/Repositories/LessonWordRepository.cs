using Core.Domain.Entity;
using Core.Persistence;
using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Configuration;
using Infrastructure.Persistence.Sqlite.Models;

namespace Infrastructure.Persistence.Sqlite.Repositories
{
    internal class LessonWordRepository : ILessonWordRepository
    {
        private readonly IInfrastructureMapper<LessonWord, LessonWordModel> _mapper;
        private readonly DbConnection _connection;
        public LessonWordRepository(
            DbConnection connection,
            IInfrastructureMapper<LessonWord, LessonWordModel> mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }

        public async Task AddWordsToLesson(int lessonId, IEnumerable<int> wordIds)
        {
            foreach (var word in wordIds)
            {
                await Create(new LessonWord
                {
                    LessonId = lessonId,
                    WordId = word
                });
            }
        }

        public async Task<LessonWord> Create(LessonWord createdLessonWord)
        {
            await _connection.Init();

            var lessonModel = _mapper.MapToModel(createdLessonWord);

            await _connection.Database.InsertAsync(lessonModel);

            return _mapper.MapToDomain(lessonModel);
           
        }

        public async Task<int> Delete(int lessenId)
        {
            await _connection.Init();
            await _connection.Database.DeleteAsync(lessenId);
            return lessenId;
        }
    }
}
