using SQLite;

namespace Infrastructure.Persistence.Sqlite.Models
{
    public class RuleModel
    {
        [PrimaryKey]
        public int RuleId { get; set; }
        public string Title { get; set; }
        [Ignore]
        public List<GrammarBlockModel> GrammarBlocks { get; set; } = [];

    }
}
