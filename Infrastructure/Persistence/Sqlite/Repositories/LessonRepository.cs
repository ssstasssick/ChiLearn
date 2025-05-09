using Core.Domain.Entity;
using Core.Persistence;
using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Configuration;
using Infrastructure.Persistence.Sqlite.Models;
using System.ComponentModel.DataAnnotations;


namespace Infrastructure.Persistence.Sqlite.Repositories
{
    internal class LessonRepository : ILessonRepository
    {
        private readonly DbConnection _connection;
        private readonly IInfrastructureMapper<Lesson, LessonModel> _mapper;
        public LessonRepository(
            DbConnection connection, 
            IInfrastructureMapper<Lesson, LessonModel> mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }

        public async Task<Lesson> Create(Lesson lesson)
        {
            await _connection.Init();

            var lessonModel = _mapper.MapToModel(lesson);

            await _connection.Database.
                InsertAsync(lessonModel);

            return _mapper.MapToDomain(lessonModel);
                
                
        }

        public async Task<int> Delete(int lessonId)
        {
            await _connection.Init();

            await _connection.Database.
                Table<LessonModel>().
                DeleteAsync(l => l.LessonId == lessonId);

            return lessonId;
        }

        public async Task<List<Lesson>> GetAll()
        {
            await _connection.Init();

            var lessons = await _connection.Database.
                Table<LessonModel>().
                ToListAsync();

            var domeinLessons = lessons.
                Select(_mapper.MapToDomain).
                ToList();   

            return domeinLessons;
        }

        public async Task<Lesson?> GetById(int lessonId)
        {
            await _connection.Init();

            var lesson = await _connection.Database
                .Table<LessonModel>()
                .FirstOrDefaultAsync(l => l.LessonId.Equals(lessonId));

            return lesson is not null
                ? _mapper.MapToDomain(lesson)
                : null;
        }

        public async Task<Lesson> Update(Lesson entity)
        {
            await _connection.Init();

            var lessonModel = _mapper.MapToModel(entity);

            await _connection.Database
                .UpdateAsync(lessonModel);

            return entity;
        }

    }
}
