using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistence
{
    public interface ILessonWordRepository
    {
        Task AddWordsToLesson(int lessonId, IEnumerable<int> wordIds);
        Task<IEnumerable<int>> GetWordsIdsByLessonId(int lessonId);
        Task<int> Delete(int lessonId);
        Task<LessonWord> Create(LessonWord CreatedLessonWord);
    }
}
