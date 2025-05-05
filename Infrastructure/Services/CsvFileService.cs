using ChiLearn.Infrastructure;
using Core.Domain.Entity;
using Core.Domain.Services;
using Core.Persistence;
using CsvHelper;
using CsvHelper.Configuration;
using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Configuration;
using Infrastructure.Persistence.Sqlite.Models;
using Infrastructure.Persistence.Sqlite.Repositories;
using System.Data.Common;
using System.Globalization;
using System.Reflection.Metadata;

namespace Infrastructure.Services
{
    internal class CsvFileService : ICsvDataService
    {
        private readonly InfrastuctureConfiguration _configuration;
        private readonly IWordRepository _wordRepository;
        private readonly ILessonWordRepository _lessonWordRepository;
        private readonly IInfrastructureMapper<Word, WordModel> _mapper;
        
        public CsvFileService(
            InfrastuctureConfiguration configuration,
            IWordRepository wordRepository,
            ILessonWordRepository lessonWordRepository,
            IInfrastructureMapper<Word, WordModel> mapper)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _wordRepository = wordRepository;
            _lessonWordRepository = lessonWordRepository;
            _mapper = mapper;
        }
        private List<Word> GetWordsFromCsv(int hskLevel)
        {
            string csvPath = Path.Combine(_configuration.CsvDirectoryPath, Constants.HskCsvFileName[hskLevel]);
            if (File.Exists(csvPath))
            {
                using var reader = new StreamReader(csvPath);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.ToLower(),
                    MissingFieldFound = null
                };
                using var csv = new CsvReader(reader, config);
                var words = csv.GetRecords<WordModel>();
                return words.Select(_mapper.MapToDomain).ToList();
            }
            return null;

        }

        public async Task SeedDatabase()
        {
            for (int level = 1; level <= Constants.HskCsvFileName.Count; level++)
            {
                await SeedWords(level);
                await SeedLessons(level);
            }
        }

        private async Task SeedWords(int hskLevel)
        {
            var words = GetWordsFromCsv(hskLevel);

            if (words == null || !words.Any())
            {
                throw new InvalidOperationException($"No words found in CSV for HSK level {hskLevel}");
            }

            foreach (var word in words)
            {
                await _wordRepository.Create(word);
            }
        }

        private async Task SeedLessons(int wordsInLesson)
        {
            for (int hskLevel = 1; hskLevel <= Constants.HskCsvFileName.Count; hskLevel++)
            {
                var words = await _wordRepository.GetWordsByHskLevel(hskLevel);
                var wordIds = words.Select(w => w.WordId).ToList();

                for (int i = 0; i < wordIds.Count; i += wordsInLesson)
                {
                    var lesson = new LessonModel {
                        HskLevel = hskLevel,
                        LessonNum = i / wordsInLesson + 1,
                        Description = $"Слова {i + 1}-{Math.Min(i + wordsInLesson, wordIds.Count)} уровня HSK {hskLevel}"};

                    await _lessonWordRepository.AddWordsToLesson(lesson.LessonId, wordIds.Skip(i).Take(20));
                }
            }
        }
    }
}
