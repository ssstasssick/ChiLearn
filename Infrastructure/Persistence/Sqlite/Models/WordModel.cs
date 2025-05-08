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
        [CsvHelper.Configuration.Attributes.Ignore]
        public int WordId { get; set; }
        [Name("Chinese")]
        public string ChiWord { get; set; }
        [Name("Russian")]
        public string RuWord { get; set; }
        [Name("English")]
        public string EngWord { get; set; }
        [Name("Pinyin")]
        public string Pinyin { get; set; }
        [CsvHelper.Configuration.Attributes.Ignore]
        public bool Learned { get; set; } = false;
        [CsvHelper.Configuration.Attributes.Ignore]
        public int HskLevel { get; set; }
        [CsvHelper.Configuration.Attributes.Ignore]
        public string? AudioPath { get; set; }
    }
}
