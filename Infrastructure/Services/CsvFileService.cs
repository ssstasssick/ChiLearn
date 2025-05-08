using ChiLearn.Infrastructure;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using CsvHelper;
using CsvHelper.Configuration;
using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Configuration;
using Infrastructure.Persistence.Sqlite.Models;
using System.Globalization;
using System.Reflection;

namespace Infrastructure.Services
{
    internal class CsvFileService : ICsvDataService
    {
        private readonly IInfrastructureMapper<Word, WordModel> _mapper;
        private readonly InfrastuctureConfiguration _configuration;


        public CsvFileService(
            IInfrastructureMapper<Word, WordModel> mapper,
            InfrastuctureConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }
        public List<Word> GetWordsFromCsv(int hskLevel)
        {

            string csvPath = Path.Combine(_configuration.AppDirectoryPath, "Resources", "Raw", Constants.HskCsvFileName[hskLevel]);

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

        
    }
}
