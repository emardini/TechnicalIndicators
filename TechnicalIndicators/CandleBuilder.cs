using System;
namespace TechnicalIndicators
{
    public class CandleBuilderEventArgs : EventArgs
    {
        public CandleBuilderEventArgs(Candle newCandle)
        {
            this.newCandle = newCandle;
        }
        private Candle newCandle;
        public Candle NewCandle
        {
            get { return newCandle; }
        }
    }

    public class CandleBuilder
    {
        private decimal? high;
        private decimal? low;
        private decimal? open;
        private decimal? close;
        private DateTime? timestamp;

        public Candle CurrentCandle
        {
            get { return new Candle(open.GetValueOrDefault(), high.GetValueOrDefault(), low.GetValueOrDefault(), close.GetValueOrDefault(), timestamp.GetValueOrDefault()); }
        }

        private readonly TimeSpan period;

        public CandleBuilder(TimeSpan period)
        {
            if(period == null)
            {
                throw new ArgumentException(nameof(period));
            }

            this.period = period;
        }

        public void AddRate(Rate newRate)
        {
            if(newRate == null)
            {
                throw new ArgumentNullException(nameof(newRate));
            }

            var roundedTime = newRate.Time.RoundDown(period);
            if(roundedTime < this.timestamp)
            {
                throw new Exception("Data stream is not advancing in time");
            }

            if(timestamp == null)
            {
                this.timestamp = roundedTime;
                this.open = newRate.MidPoint;
            }

            if(roundedTime > this.timestamp)
            {
                OnNewCandleCreated(new CandleBuilderEventArgs(CurrentCandle));
                this.timestamp = roundedTime;
                this.open = newRate.MidPoint;
            }

            this.high = newRate.MidPoint >= this.high.GetValueOrDefault(newRate.MidPoint) ? newRate.MidPoint : high;
            this.low = newRate.MidPoint <= this.low.GetValueOrDefault(newRate.MidPoint) ? newRate.MidPoint :  low;
            this.close = newRate.MidPoint;
        }

        public event EventHandler<CandleBuilderEventArgs> NewCandleCreated;

        protected virtual void OnNewCandleCreated(CandleBuilderEventArgs e)
        {
            NewCandleCreated?.Invoke(this, e);
        }
    }
}
