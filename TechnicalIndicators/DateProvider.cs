﻿namespace TechnicalIndicators
{
    using System;

    public class SimpleDateProvider : IDateProvider
    {
        #region Public Methods and Operators

        public DateTime GetCurrentUtcDate()
        {
            return DateTime.UtcNow;
        }

        #endregion
    }
}