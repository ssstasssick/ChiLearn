using SQLite;

namespace Infrastructure.Persistence.Sqlite.Models
{
    internal class LessonModel
    {
        [PrimaryKey, AutoIncrement]
        public int LessonId { get; set; }
        public int LessonNum { get; set; }
        public int? HskLevel { get; set; }
        public string? Description { get; set; }
        public bool CompletedTheory { get; set; } = false;
        public bool CompletedPractice { get; set; } = false;
       
    }
}
