using Core.Domain.Abstractions.Sevices;
using Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services
{
    public class WordService : IWordService
    {
        private readonly IWordRepository _wordRepository;
        public WordService(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }
        public async Task ChangeWordFavoritesState(int id)
        {
            var word = await _wordRepository.GetById(id);
            word.IsFavorite = !word.IsFavorite;
            await _wordRepository.Update(word);
        }

        public async Task ResetFavorites()
        {
            var favoritesWords = (await _wordRepository.GetAll())
                .Where(w => w.IsFavorite.Equals(true));
            foreach (var word in favoritesWords)
            {
                word.IsFavorite = false;
                await _wordRepository.Update(word);
            }
        }
    }
}
