using Core.Domain.Entity;

namespace Core.Persistence
{
    public interface IGrammarBlockRepository
    {
        Task<GrammarBlock> Create(GrammarBlock block);
        Task<List<GrammarBlock>> GetByRuleId(int ruleId);
    }
}
