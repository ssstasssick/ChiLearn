using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiLearn.Models
{
    public class HskLevelItem
    {
        public int Level { get; }
        public string DisplayName { get; }

        public HskLevelItem(int level, string displayName)
        {
            Level = level;
            DisplayName = displayName;
        }
    }
}
