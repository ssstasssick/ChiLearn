

namespace Core.Domain.Entity
{
    public class Word
    {
        public int WordId { get; set; }
        public string ChiWord { get; set; }
        public string RuWord { get; set; }    
        public string EngWord { get; set; }
        public string Pinyin { get; set; }
        public bool Learned { get;set; }// flag: 0 - not, 1 - learned
        public int HskLevel { get; set; }
        public string? AudioPath { get; set; }
        public List<Lesson> Lessons { get; } = [];
        public Word() { }
        
    }
}
