namespace Core.Domain.Entity
{
    public class LessonWord
    {
        public int LessonWordId { get; set; }
        public int LessonId { get; set; }
        public int WordId { get; set; }
        public LessonWord() { }

    }
}
