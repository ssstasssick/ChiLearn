﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Abstractions.Sevices
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync();
    }
}
