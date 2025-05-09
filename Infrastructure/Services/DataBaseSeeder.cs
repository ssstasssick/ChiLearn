using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Core.Persistence;
using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Configuration;
using Infrastructure.Persistence.Sqlite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    internal class DataBaseSeeder : IDataBaseSeeder
    {
        private readonly IWordRepository _wordRepository;
        private readonly ILessonWordRepository _lessonWordRepository;
        private readonly ICsvDataService _csvReaderService;
        private readonly ILessonRepository _lessonRepository;
        private readonly IInfrastructureMapper<Lesson, LessonModel> _infrastructureMapper;

        public DataBaseSeeder(
        IWordRepository wordRepository,
        ILessonRepository lessonRepository,
        ILessonWordRepository lessonWordRepository,
        ICsvDataService csvReaderService,
        IInfrastructureMapper<Lesson, LessonModel> infrastructureMapper)
        {
            _wordRepository = wordRepository;
            _lessonWordRepository = lessonWordRepository;
            _csvReaderService = csvReaderService;
            _lessonRepository = lessonRepository;
            _infrastructureMapper = infrastructureMapper;
        }

        public async Task SeedDatabase()
        {
            for (int level = 1; level <= Constants.HskCsvFileName.Count; level++)
            {
                await SeedWords(level);
                await SeedLessons(10);
            }   
        }

        private async Task SeedWords(int hskLevel)
        {
            if (await _wordRepository.AnyAsync() is false)
            {
                var words = _csvReaderService.GetWordsFromCsv(hskLevel);

                if (words.Count.Equals(0))
                {
                    throw new InvalidOperationException($"No words found in CSV for HSK level {hskLevel}");
                }

                words.ForEach(w =>
                {
                    w.HskLevel = hskLevel;
                    var audioFileName = $"{ConverterChiWordToUnicode.ConvertToUnicode(w.ChiWord)}";
                    w.AudioPath = audioFileName;
                });

                foreach (var word in words)
                {
                    await _wordRepository.Create(word);
                }
            }
        }

        private async Task SeedLessons(int wordsInLesson)
        {
            if (await _lessonRepository.AnyAsync() is false)
            {
                for (int hskLevel = 1; hskLevel <= Constants.HskCsvFileName.Count; hskLevel++)
                {
                    var words = await _wordRepository.GetWordsByHskLevel(hskLevel);
                    var wordIds = words.Select(w => w.WordId).ToList();

                    for (int i = 0; i < wordIds.Count; i += wordsInLesson)
                    {
                        var lesson = new Lesson
                        {
                            HskLevel = hskLevel,
                            LessonNum = i / wordsInLesson + 1,
                            Description = $"Слова {i + 1}-{Math.Min(i + wordsInLesson, wordIds.Count)} уровня HSK {hskLevel}"
                        };
                        lesson = await _lessonRepository.Create(lesson);
                        await _lessonWordRepository.AddWordsToLesson(lesson.LessonId, wordIds.Skip(i).Take(wordsInLesson));
                    }
                }
            }
        }
    }
}
