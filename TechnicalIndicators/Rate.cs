namespace TechnicalIndicators
{
    using System;

    public class Rate
    {
        #region Public Properties

        public decimal Ask { get; set; }

        public string BaseCurrency
        {
            get { return this.Instrument.Safe().PadRight(7, ' ').Substring(0, 3); }
        }

        public decimal Bid { get; set; }

        public string Instrument { get; set; }

        public string QuoteCurrency
        {
            get { return this.Instrument.Safe().PadRight(7, ' ').Substring(4, 3); }
        }

        public DateTime Time { get; set; }

        #endregion
    }
}