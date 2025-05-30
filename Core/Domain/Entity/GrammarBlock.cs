﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entity
{
    public class GrammarBlock
    {
        public int RuleId { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Ch { get; set; }
        public string Pn { get; set; }
        public string Rus { get; set; }
        public List<List<string>> Rows { get; set; }
    }
}
