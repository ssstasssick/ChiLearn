﻿using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistence
{
    public interface ILessonRepository : IRepository<Lesson>
    {
        Task<bool> AnyAsync(int hskLevel);
        
    }
}
