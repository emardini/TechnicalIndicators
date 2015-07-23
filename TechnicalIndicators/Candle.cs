namespace TechnicalIndicators
{
    using System;

    public class Candle
    {
        public DateTime Time { get; set; }

        public decimal OpenBid { get; set; }

        public decimal HighBid { get; set; }

        public decimal LowBid { get; set; }

        public decimal CloseBid { get; set; }

        public decimal OpenAsk { get; set; }

        public decimal HighAsk { get; set; }

        public decimal LowAsk { get; set; }

        public decimal CloseAsk { get; set; }

        public bool IsAskDown
        {
            get { return this.CloseAsk <= this.OpenAsk; }
        }

        public bool IsAskUp
        {
            get { return this.CloseAsk > this.OpenAsk; }
        }

        public bool IsBidDown
        {
            get { return this.CloseBid > this.OpenBid; }
        }

        public bool IsBidUp
        {
            get { return this.CloseBid <= this.OpenBid; }
        }

        public bool IsAskReversal(Threshold threshold)
        {
            if (threshold == null)
            {
                throw new ArgumentNullException("threshold");
            }

            if (CloseAsk - OpenAsk <= threshold.Delta) return true;

            if(CloseAsk - OpenAsk < threshold.Body * (HighAsk - LowAsk))  return true;

            return false;
        }
    }
}