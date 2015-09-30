namespace TechnicalIndicators
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Extensions
    {
        #region Public Methods and Operators

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> list, int nbOfItems)
        {
            return list.Reverse().Take(nbOfItems);
        }

        #endregion
    }
}