using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services
{
    public class NotebookService : INotebookService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ILessonService _lessonService;

        public NotebookService(
            ILessonRepository lessonRepository,
            ILessonService lessonService,
            IWordRepository wordRepository)
        {
            _lessonRepository = lessonRepository;
            _lessonService = lessonService;
        }

        public async Task<List<Word>> GetLearnedWords()
        {
            var completedLessons = (await _lessonRepository.GetAll())
                .Where(l => l.CompletedPractice == true)
                .ToList();

            var learnedWords = new List<Word>();

            foreach (var lesson in completedLessons)
            {
                var fullLesson = await _lessonService.GetLessonsById(lesson.LessonId);
                if (fullLesson.Words != null)
                    learnedWords.AddRange(fullLesson.Words);
            }

            return learnedWords.ToList();
        }


    }
}
