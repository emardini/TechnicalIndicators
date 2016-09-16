namespace TechnicalIndicators
{
    using System;

    public class Order
    {
        #region Public Properties

        public string AcountId { get; set; }

        public DateTime? Expiry { get; set; }

        public string Instrument { get; set; }

        public decimal LowerBound { get; set; }

        public string OrderType { get; set; }

        public decimal Price { get; set; }

        public decimal StopLoss { get; set; }

        public decimal TakeProfit { get; set; }

        public decimal TrailingStop { get; set; }

        public int Units { get; set; }

        public decimal UpperBound { get; set; }

        public string Side { get; set; }

        public DateTime Timestamp { get; set; }

        #endregion
    }
}