using System.Collections.Generic;
using System.Linq;

namespace TechnicalIndicators
{
    public static class Extensions
    {
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> list, int nbOfItems)
        {
            return list.Reverse().Take(nbOfItems);
        }
    }
}
