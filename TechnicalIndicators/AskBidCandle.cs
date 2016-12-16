namespace TechnicalIndicators
{
    using System;

    public class AskBidCandle
    {
        #region Public Properties

        public int Id
        {
            get;set;
        }

        public decimal CloseBid
        {
            get; set;
        }

        public decimal HighBid
        {
            get; set;
        }

        public bool IsDown
        {
            get { return this.CloseBid <= this.OpenBid; }
        }

        public bool IsUp
        {
            get { return this.CloseBid > this.OpenBid; }
        }
        public decimal LowBid
        {
            get; set;
        }

        public decimal OpenBid
        {
            get;set;
        }

        public decimal CloseAsk
        {
            get; set;
        }

        public decimal HighAsk
        {
            get; set;
        }

        public decimal LowAsk
        {
            get; set;
        }
        public decimal OpenAsk
        {
            get; set;
        }

        public DateTime UTCTickDateTime
        {
            get;set;
        }

        public int Ticks
        {
            get;set;
        }

        #endregion
    }


}