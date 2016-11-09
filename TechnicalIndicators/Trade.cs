namespace TechnicalIndicators
{
    using System;

    public class Trade
    {
        #region Public Properties

        public string QuoteCurrency
        {
            get { return this.Instrument.Safe().PadRight(7, ' ').Substring(4, 3); }
        }

        public string BaseCurrency
        {
            get { return this.Instrument.Safe().PadRight(7, ' ').Substring(0, 3); }
        }

        public long Id { get; set; }

        public string Instrument { get; set; }

        public decimal Price { get; set; }

        public string Side { get; set; }

        public decimal StopLoss { get; set; }

        public decimal TakeProfit { get; set; }

        public DateTime Time { get; set; }

        public decimal TrailingAmount { get; set; }

        public decimal TrailingStop { get; set; }

        public int Units { get; set; }

        public string AccountId { get; set; }

        #endregion
    }
}