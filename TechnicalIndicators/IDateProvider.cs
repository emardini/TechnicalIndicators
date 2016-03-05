namespace TechnicalIndicators
{
    using System;

    public interface IDateProvider
    {
        #region Public Methods and Operators

        DateTime GetCurrentUtcDate();

        #endregion
    }
}