using Core.Domain.Entity;
using CsvHelper.Configuration.Attributes;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Sqlite.Models
{
    internal class WordModel
    {
        [PrimaryKey, AutoIncrement]
        public int WordId { get; set; }
        [Name("Chinese")]
        public string ChiWord { get; set; }
        [Name("Russian")]
        public string RuWord { get; set; }
        [Name("English")]
        public string EngWord { get; set; }
        [Name("Pinyin")]
        public string Pinyin { get; set; }
        public bool Learned { get; set; } = false;
        public int HskLevel { get; set; }
        public string? AudioPath { get; set; }
    }
}
