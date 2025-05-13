

namespace Core.Domain.Entity
{
    public class Word
    {
        public int WordId { get; set; }
        public string ChiWord { get; set; }
        public string RuWord { get; set; }    
        public string EngWord { get; set; }
        public string Pinyin { get; set; }
        public bool IsFavorite { get;set; }
        public int HskLevel { get; set; }
        public string? AudioPath { get; set; }
        public Word() { }
        
    }
}
