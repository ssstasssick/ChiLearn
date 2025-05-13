using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Abstractions.Sevices
{
    public interface ILessonService
    {
        Task<Lesson> GetLessonsById(int lessonId);
        Task<List<Lesson>> GetAllLessons();
        Task UpdateLesson(Lesson lesson);
        Task<Lesson?> GetLastCompletedLessonAsync();
        Task<int> GetCountOfLessonsByHskLevel(int hskLvl);


    }
}
