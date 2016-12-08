namespace TechnicalIndicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Extensions
    {
        #region Public Methods and Operators

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> list, int nbOfItems)
        {
            return list.Reverse().Take(nbOfItems);
        }

        public static decimal GetPipFraction(this string quoteCurrency)
        {
            return quoteCurrency == "JPY" ? 0.01m : 0.0001m;
        }

        /// <summary>
        /// Extension method to round a datetime down by a timespan interval.
        /// </summary>
        /// <param name="dateTime">Base DateTime object we're rounding down.</param>
        /// <param name="interval">Timespan interval to round to.</param>
        /// <returns>Rounded datetime</returns>
        public static DateTime RoundDown(this DateTime dateTime, TimeSpan interval)
        {
            if (interval == TimeSpan.Zero)
            {
                // divide by zero exception
                return dateTime;
            }
            return dateTime.AddTicks(-(dateTime.Ticks % interval.Ticks));
        }

        #endregion
    }
}