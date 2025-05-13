using SQLite;

namespace Infrastructure.Persistence.Sqlite.Models
{
    internal class RuleModel
    {
        [PrimaryKey]
        public int RuleId { get; set; }
        [NotNull, Unique]
        public string Title { get; set; }
        [NotNull, Unique]
        public string RuleText { get; set; }
        public string? Example { get; set; }
        public string? ImgPath { get; set; }
        public bool IsFavorite { get; set; } = false;

    }
}
