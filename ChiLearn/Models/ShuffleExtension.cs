using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiLearn.Models
{
    public static class ShuffleExtension
    {
        private static Random rng = new();

        public static void Shuffle<T>(this List<T> collection)
        {
            var list = collection.ToList();
            collection.Clear();
            foreach (var item in list.OrderBy(_ => rng.Next()))
                collection.Add(item);
        }
    }
}
