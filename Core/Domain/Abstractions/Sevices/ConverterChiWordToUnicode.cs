using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Abstractions.Sevices
{
    public static class ConverterChiWordToUnicode
    {
        public static string ConvertToUnicode(string chineseWord)
        {
            StringBuilder result = new StringBuilder();

            foreach (char c in chineseWord)
            {
                if (c > 127) 
                {
                    result.Append($"U{(int)c:X4}");
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
    }
}
