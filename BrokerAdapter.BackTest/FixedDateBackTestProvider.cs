﻿namespace BrokerAdapter.BackTest
{
    using System;

    using TechnicalIndicators;

    public class FixedDateBackTestProvider : IDateProvider
    {
        #region Public Methods and Operators

        public DateTime GetCurrentDate()
        {
            return new DateTime(2016, 5, 11, 21, 0, 0);
        }

        public DateTime GetCurrentUtcDate()
        {
            return this.GetCurrentDate();
        }

        #endregion
    }
}