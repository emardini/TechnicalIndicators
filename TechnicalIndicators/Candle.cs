namespace TechnicalIndicators
{
    using System;

    public class Candle
    {
        #region Fields

        private readonly decimal close;

        private readonly decimal high;

        private readonly decimal low;

        private readonly decimal open;

        #endregion

        #region Constructors and Destructors

        public Candle() : this(0, 0, 0, 0)
        {
        }

        public Candle(decimal open, decimal high, decimal low, decimal close)
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

            if (low > open)
            {
                throw new ArgumentException("Low cannot be greater than open");
            }

            if (low > close)
            {
                throw new ArgumentException("Low cannot be greater than close");
            }

            this.open = open;
            this.close = close;
            this.low = low;
            this.high = high;

            if (this.BodyRange > this.FullRange)
            {
                throw new ArgumentException("The body range cannot be greater than the full range");
            }
        }

        #endregion

        #region Public Properties

        public decimal BodyRange
        {
            get { return Math.Abs(this.open - this.close); }
        }

        public decimal Close
        {
            get { return this.close; }
        }

        public decimal FullRange
        {
            get { return this.High - this.Low; }
        }
        public decimal High
        {
            get { return this.high; }
        }

        public bool IsDown
        {
            get { return this.Close <= this.Open; }
        }

        public bool IsUp
        {
            get { return this.Close > this.Open; }
        }
        public decimal Low
        {
            get { return this.low; }
        }
        public decimal Open
        {
            get { return this.open; }
        }

        #endregion

        #region Public Methods and Operators

        public bool IsReversal(Threshold threshold)
        {
            if (threshold == null)
            {
                throw new ArgumentNullException("threshold");
            }

            return (this.Close - this.Open <= threshold.Delta);
        }

        #endregion
    }
}