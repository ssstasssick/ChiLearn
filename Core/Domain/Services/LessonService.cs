using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services
{
    internal class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IWordRepository _wordRepository;
        private readonly ILessonWordRepository _lessonWordRepository;
        public LessonService(
            ILessonRepository lessonRepository,
            IWordRepository wordRepository,
            ILessonWordRepository lessonWordRepository)
        {
            _lessonRepository = lessonRepository;
            _wordRepository = wordRepository;
            _lessonWordRepository = lessonWordRepository;
        }

        public async Task UpdateLesson(Lesson updatedLesson)
        {
            await _lessonRepository.Update(updatedLesson);
        }

        public async Task<List<Lesson>> GetAllLessons()
        {
            var lessons = await _lessonRepository.GetAll();
            return lessons;
        }

        public async Task<Lesson> GetLessonsById(int lessonId)
        {
            var lessonById = await _lessonRepository.GetById(lessonId);
            var wordsIds = await _lessonWordRepository.GetWordsIdsByLessonId(lessonId);
            lessonById.Words = await _wordRepository.GetWordsByIds(wordsIds);

            return lessonById;

        }

        public async Task<Lesson?> GetLastCompletedLessonAsync()
        {
            var allLessons = await _lessonRepository.GetAll();

            var completedLesson = allLessons
                .Where(l => l.CompletedTheory && l.CompletedPractice)
                .OrderByDescending(l => l.LessonNum)
                .FirstOrDefault();

            return completedLesson;
        }

        public async Task<int> GetCountOfLessonsByHskLevel(int hskLvl)
        {
            var allLessons = await _lessonRepository.GetAll();
            return allLessons
                .Where(all => all.HskLevel.Equals(hskLvl))
                .Count();
        }

        public async Task UpdateLastLevel(int levelNum)
        {
            var allLessons = await _lessonRepository.GetAll();

            var lessonsToUpdate = allLessons
                .Where(l => l.LessonNum < levelNum)
                .ToList();

            foreach (var lesson in lessonsToUpdate)
            {
                lesson.CompletedTheory = true;
                lesson.CompletedPractice = true;
                await _lessonRepository.Update(lesson);
            }
        }

        public async Task ResetCompletedLevels()
        {
            var allLessons = await _lessonRepository.GetAll();

            foreach (var lesson in allLessons)
            {
                lesson.CompletedTheory = false;
                lesson.CompletedPractice = false;
                await _lessonRepository.Update(lesson);
            }

            
        }
    }
}
