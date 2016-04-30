namespace TechnicalIndicators
{
    using System;

    public class SimpleDateProvider : IDateProvider
    {
        #region Public Methods and Operators

        public DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }

        public DateTime GetCurrentUtcDate()
        {
            return DateTime.UtcNow;
        }

        #endregion
    }
}