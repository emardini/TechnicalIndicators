namespace TechnicalIndicators
{
    using System;
    using System.Globalization;

    /// <summary>
    /// </summary>
    public static class StringParserExtensions
    {
        #region Public Methods and Operators
        /// <summary>
        ///     Parses an string to int with invariant culture
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static string Safe(this string that)
        {
            return that ?? string.Empty;
        }

        /// <summary>
        ///     Parses an string to bool with invariant culture
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static bool? SafeParseBool(this string that)
        {
            bool test;
            return bool.TryParse(that, out test) ? test : (bool?)null;
        }

        /// <summary>
        ///     Parses an string to datetime with invariant culture
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static DateTime? SafeParseDate(this string that)
        {
            DateTime test;
            return DateTime.TryParse(that, out test) ? test : (DateTime?)null;
        }

        /// <summary>
        ///     Parses an string to double with invariant culture
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static double? SafeParseDouble(this string that)
        {
            double test;
            return double.TryParse(that, NumberStyles.Float, CultureInfo.InvariantCulture, out test) ? test : (double?)null;
        }

        /// <summary>
        ///     Parses an string to double with invariant culture
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static decimal? SafeParseDecimal(this string that)
        {
            decimal test;
            return decimal.TryParse(that, NumberStyles.Float, CultureInfo.InvariantCulture, out test) ? test : (decimal?)null;
        }

        /// <summary>
        ///     Parses an string to float with invariant culture
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static double? SafeParseFloat(this string that)
        {
            float test;
            return float.TryParse(that, NumberStyles.Float, CultureInfo.InvariantCulture, out test) ? test : (float?)null;
        }

        /// <summary>
        ///     Parses an string to int with invariant culture
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static int? SafeParseInt(this string that)
        {
            int test;
            return int.TryParse(that, NumberStyles.Integer, CultureInfo.InvariantCulture, out test) ? test : (int?)null;
        }

        /// <summary>
        ///     Parses an string to long with invariant culture
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static long? SafeParseLong(this string that)
        {
            long test;
            return long.TryParse(that, NumberStyles.Integer, CultureInfo.InvariantCulture, out test) ? test : (long?)null;
        }

        /// <summary>
        ///     Parses an string to timespan with invariant culture
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static TimeSpan? SafeParseTimespan(this string that)
        {
            TimeSpan test;
            return TimeSpan.TryParse(that, out test) ? test : (TimeSpan?)null;
        }

        #endregion
    }
}