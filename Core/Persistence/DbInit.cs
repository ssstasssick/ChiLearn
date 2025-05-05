using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistence
{
    internal interface IDbDataInitializer
    {
        Task SeedDataAsync();    
    }
}
