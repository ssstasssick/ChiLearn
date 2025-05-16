using Core.Domain.Entity;
using Core.Persistence;
using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Configuration;
using Infrastructure.Persistence.Sqlite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Sqlite.Repositories
{
    internal class WordRepository : IWordRepository
    {
        private readonly DbConnection _connection;
        private readonly IInfrastructureMapper<Word, WordModel> _mapper;

        public WordRepository(
            DbConnection connection,
            IInfrastructureMapper<Word, WordModel> mapper) 
        {
            _connection = connection;
            _mapper = mapper;
        }

        public async Task<bool> AnyAsync(int hskLevel)
        {
            await _connection.Init();

            var exists = await _connection.Database
                .Table<WordModel>()
                .Where(w => w.HskLevel.Equals(hskLevel))
                .CountAsync();

            return exists > 0;
        }

        public async Task<Word> Create(Word lesson)
        {
            await _connection.Init();
            var lessonModel = _mapper.MapToModel(lesson);
            await _connection.Database.InsertAsync(lessonModel);
            return _mapper.MapToDomain(lessonModel);

        }

        public Task<int> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Word>> GetAll()
        {
            await _connection.Init();
            var wordsList = await _connection.Database
                .Table<WordModel>()
                .ToListAsync();
            var wordsModelList = wordsList
                .Select(w => _mapper.MapToDomain(w))
                .ToList();

            return wordsModelList;
        }

        public async Task<Word?> GetById(int id)
        {
            await _connection.Init();
            var word = await _connection.Database
                .Table<WordModel>()
                .FirstOrDefaultAsync(w => w.WordId.Equals(id));

            return word is not null 
                ? _mapper.MapToDomain(word) 
                : null;
        }

        public async Task<List<Word>> GetWordsByHskLevel(int hslLevel)
        {
            await _connection.Init();

            var wordsModel = await _connection.Database
                .Table<WordModel>()
                .Where(w => w.HskLevel == hslLevel)
                .ToListAsync();

            return wordsModel.
                Select(_mapper.MapToDomain).ToList();
        }

        public async Task<List<Word>> GetWordsByIds(IEnumerable<int> ids)
        {
            await _connection.Init();
            var wordsModels = await _connection.Database
                .Table<WordModel>()
                .Where(w => ids.Contains(w.WordId))
                .ToArrayAsync();

            return wordsModels
                .Select(_mapper.MapToDomain)
                .ToList();
           
        }

        public async Task<Word> Update(Word lesson)
        {
            await _connection.Init();

            var lessonModel = _mapper.MapToModel(lesson);

            await _connection.Database
                .UpdateAsync(lessonModel);

            return lesson;
        }
    }
}
