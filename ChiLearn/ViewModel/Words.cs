using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChiLearn.ViewModel
{
    internal class Words
    {
        public Guid Word_id {  get; set; }
        public string Chi_word { get; set; }
        public string Ru_word { get; set; }
        public string Pinyin { get; set; }
        public int Hsk_lvl{ get; set; }
        public string Audio_path { get; set; }

        public Words(Guid word_id, string chi_word, string ru_word, string pinyin, int hsk_lvl, string aydio_path)
        {
            Word_id = word_id;
            Chi_word = chi_word;
            Ru_word = ru_word;
            Pinyin = pinyin;
            Hsk_lvl = hsk_lvl;
            Audio_path = aydio_path;
        }


    }
}
