
using SQLite;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Sqlite.Models
{
    internal class LessonWordModel
    {
        [PrimaryKey, AutoIncrement]
        public int LessonWordId { get; set; }
        [Indexed( Name = "IX_LessonWord", Unique = true, Order = 2)]
        public int WordId { get; set; }
        [Indexed(Name = "IX_LessonWord", Unique = true, Order = 1)]
        public int LessonId { get; set; }

    }
}
