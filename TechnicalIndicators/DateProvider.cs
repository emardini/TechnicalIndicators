namespace TechnicalIndicators
{
    using System;

    public class SimpleDateProvider : IDateProvider
    {
        #region Public Methods and Operators

        public DateTime GetCurrentEastDateTimeDate()
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Eastern Standard Time");
        }

        public DateTime GetCurrentUtcDate()
        {
            return DateTime.UtcNow;
        }

        #endregion
    }
}