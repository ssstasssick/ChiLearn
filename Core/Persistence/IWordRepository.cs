using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistence
{
    public interface IWordRepository : IRepository<Word>
    {
        Task<List<Word>> GetWordsByHskLevel(int hslLevel);
        Task<List<Word>> GetWordsByIds(IEnumerable<int> ids);
        Task<bool> AnyAsync();
    }
}
