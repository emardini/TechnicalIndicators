namespace TechnicalIndicators
{
    using System;


    public class Candle
    {
        private readonly decimal open;

        private readonly decimal close;

        private readonly decimal low;

        private readonly decimal high;

        public Candle():this(0,0,0,0){}

        public Candle(decimal open, decimal close, decimal low, decimal high)
        {
            if (open < 0)
            {
                throw new ArgumentOutOfRangeException("open", "Cannot be negative");
            }

            if (close < 0)
            {
                throw new ArgumentOutOfRangeException("close", "Cannot be negative");
            }

            if (high < 0)
            {
                throw new ArgumentOutOfRangeException("high", "Cannot be negative");
            }

            if (low < 0)
            {
                throw new ArgumentOutOfRangeException("low", "Cannot be negative");
            }

            if (low > high)
            {
                throw new ArgumentException("Low cannot be greater than high");
            }

            this.open = open;
            this.close = close;
            this.low = low;
            this.high = high;

            if (BodyRange > FullRange)
            {
                throw new ArgumentException("The body range cannot be greater than the full range");
            }
        }

        public decimal Open { get { return open; }  }

        public decimal High { get { return high; } }

        public decimal Low { get { return low; } }

        public decimal Close { get { return close; } }

        public decimal FullRange
        {
            get { return High - Low; }
        }

        public decimal BodyRange
        {
            get { return Math.Abs(open - close); }
        }

        public bool IsDown
        {
            get { return this.Close <= this.Open; }
        }

        public bool IsUp
        {
            get { return this.Close > this.Open; }
        }

        public bool IsReversal(Threshold threshold)
        {
            if (threshold == null)
            {
                throw new ArgumentNullException("threshold");
            }

            return (Close - Open <= threshold.Delta);
        }
    }
}