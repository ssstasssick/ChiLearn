namespace Core.Domain.Entity
{
    public class Lesson
    {
        public int LessonId { get; set; }
        public int LessonNum { get; set; }
        public int? HskLevel { get; set; }
        public string? Description { get; set; }
        public bool CompletedTheory { get; set; }
        public bool CompletedPractice { get; set; }
        public List<Word> Words { get; set; } = [];

        public bool IsAvailable { get; set; } = false;
        
        public Lesson() { }    


    }
}
