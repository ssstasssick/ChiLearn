using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Models;
using Core.Domain.Entity;

namespace Infrastructure.Persistence.Sqlite.Mappers
{
    internal class WordMapper : IInfrastructureMapper<Word, WordModel>
    {
        public Word MapToDomain(WordModel infrastucture)
        {
            return new Word
            {
                WordId = infrastucture.WordId,
                ChiWord = infrastucture.ChiWord,
                EngWord = infrastucture.EngWord,
                RuWord = infrastucture.RuWord,
                HskLevel = infrastucture.HskLevel,
                Learned = infrastucture.Learned,
                AudioPath = infrastucture.AudioPath,
                Pinyin = infrastucture.Pinyin
            };
        }

        public WordModel MapToModel(Word domain)
        {
            return new WordModel
            {
                WordId = domain.WordId,
                ChiWord = domain.ChiWord,
                EngWord = domain.EngWord,
                RuWord = domain.RuWord,
                HskLevel = domain.HskLevel,
                Learned = domain.Learned,
                AudioPath = domain.AudioPath,
                Pinyin = domain.Pinyin
            };
        }
    }
}
