﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entity
{
    public class Rule
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<GrammarBlock> Content { get; set; }
    }
}
