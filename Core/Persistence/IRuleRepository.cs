﻿using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistence
{
    public interface IRuleRepository
    {
        Task<bool> AnyAsync();
        Task<List<Rule>> GetRules();
        Task<Rule> Create(Rule rule);
        Task<Rule?> GetById(int ruleId);
    }
}
