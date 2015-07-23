namespace TechnicalIndicators
{
    using System;

    public class BidAskCandle
    {
        public DateTime Time { get; set; }

        public decimal OpenBid { get; set; }

        public decimal OpenAsk { get; set; }

        public decimal HighBid { get; set; }

        public decimal HighAsk { get; set; }

        public decimal LowBid { get; set; }

        public decimal LowAsk { get; set; }

        public decimal CloseBid { get; set; }

        public decimal CloseAsk { get; set; }

        public int Volume { get; set; }

        public bool Complete { get; set; }
    }
}