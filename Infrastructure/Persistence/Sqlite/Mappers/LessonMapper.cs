using Core.Domain.Entity;
using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Models;

namespace Infrastructure.Persistence.Sqlite.Mappers
{
    internal class LessonMapper : IInfrastructureMapper<Lesson, LessonModel>
    {
        public Lesson MapToDomain(LessonModel infrastucture)
        {
            var lessonDomain = new Lesson
            {
                LessonId = infrastucture.LessonId,
                LessonNum = infrastucture.LessonNum,
                HskLevel = infrastucture.HskLevel,
                Description = infrastucture.Description,
                CompletedPractice = infrastucture.CompletedPractice,
                CompletedTheory = infrastucture.CompletedTheory
            };
            return lessonDomain;
        }

        public LessonModel MapToModel(Lesson domain)
        {
            return new LessonModel
            {                
                LessonId = domain.LessonId,
                LessonNum = domain.LessonNum,
                HskLevel = domain.HskLevel,
                Description = domain.Description,
                CompletedTheory = domain.CompletedTheory,
                CompletedPractice = domain.CompletedPractice
            };
        }
    }
}
