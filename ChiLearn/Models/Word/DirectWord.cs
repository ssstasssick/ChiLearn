using ChiLearn.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiLearn.Models.Word
{
    public class DirectWord : BaseNotifyObject
    {
        public int WordId { get; set; }
        public string ChiWord { get; set; }
        public string Pinyin { get; set; }
        public string RuWord { get; set; }
        public int HskLevel { get; set; }
        public string? AudioPath { get; set; }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                if (_isFavorite != value)
                {
                    SetProperty(ref _isFavorite, value);
                }
            }
        }
    }
}
