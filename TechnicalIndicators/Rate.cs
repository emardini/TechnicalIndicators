namespace TechnicalIndicators
{
    using System;

    public class Rate
    {
        #region Public Properties

        public decimal Ask { get; set; }

        public decimal Bid { get; set; }

        public string Instrument { get; set; }

        public DateTime Time { get; set; }

        #endregion
    }
}