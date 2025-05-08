using Core.Domain.Entity;

namespace Core.Domain.Abstractions.Sevices
{
    public interface ICsvDataService
    {
        //Task SeedDatabase();
        List<Word> GetWordsFromCsv(int hskLevel);


    }
}
