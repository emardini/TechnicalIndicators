namespace TechnicalIndicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    //Use http://www.dinosaurtech.com/2007/average-directional-movement-index-adx-formula-in-c-2/
    //Or http://www.theforexguy.com/average-directional-index-indicator/
    public class Adx
    {
        private readonly int period;

        private readonly List<decimal> values;

        private readonly List<decimal> upDms;
        private readonly List<decimal> downDms;

        private readonly List<decimal> upDis;
        private readonly List<decimal> downDis;
        private readonly List<decimal> dxs;

        private readonly List<decimal> trueRanges;

        public IEnumerable<decimal> TrueRanges { get { return trueRanges; } } 

        protected const int MaxNbOfItems = 5000;

        protected const int MaxPeriod = 200;

        private readonly List<Candle> sourceValues;

        private decimal acumUpDm;
        private decimal acumTrueRange;
        private decimal acumDownDm;

        /// <summary>
        /// Initializes a new instance of the <see cref="Adx"/> class.
        /// The Adx is implemented following the classical approach with simple moving average
        /// </summary>
        /// <param name="period">The period.</param>
        public Adx(int period)
        {
            this.period = period;

            this.values = new List<decimal>();
            this.trueRanges = new List<decimal>();
            this.sourceValues = new List<Candle>();
            this.downDms = new List<decimal>();
            this.upDms = new List<decimal>();
            this.downDis = new List<decimal>();
            this.upDis = new List<decimal>();
            this.dxs = new List<decimal>();

            acumDownDm = 0;
            acumUpDm = 0;
            acumTrueRange = 0;
        }

        public void Add(Candle value)
        {
            if (!sourceValues.Any())
            {
                sourceValues.Add(value);
                return;
            }

            var lastValue = sourceValues.LastOrDefault() ?? new Candle();

            var upMove = value.High - lastValue.High;                       
            var downMove = lastValue.Low - value.Low;

            var upDm = upMove > 0 && upMove > downMove ? upMove : 0;
            var downDm = downMove > 0 && downMove > upMove ? downMove : 0;

            var trueRange = GetTrueRange(value, lastValue);

            if (sourceValues.Count() <= period)
            {
                acumDownDm = downDms.Sum();

                acumUpDm = this.upDms.Sum();

                acumTrueRange = trueRanges.Sum();
            }
            else
            {
                var smoothing = ((period - 1m) / period);

                acumDownDm = acumDownDm*smoothing + downDm;

                acumUpDm = acumUpDm * smoothing + upDm;

                acumTrueRange = acumTrueRange*smoothing + trueRange;
            }

            upDms.Add(upDm);
            downDms.Add(downDm);
            trueRanges.Add(trueRange);

            sourceValues.Add(value);

            //var downDi = acumDownDm / acumTrueRange;
            //downDis.Add(downDi);

            //var upDi = acumUpDm / acumTrueRange;
            //upDis.Add(upDi);

            //var dx = Math.Abs(upDi - downDi) / (upDi + downDi);
            //dxs.Add(dx);

            //var adx = dxs.TakeLast(period).Average()*100m;
            //values.Add(adx);
        }

        public IEnumerable<decimal> Values { get { return values; } }

        public IEnumerable<Candle> SourceValues { get { return sourceValues; } }

        public IEnumerable<decimal> UpDms
        {
            get { return this.upDms; }
        }

        public IEnumerable<decimal> DownDms
        {
            get { return this.downDms; }
        }

        protected void EnsureMaxBufferSize()
        {
            var nbItemsToRemove = MaxNbOfItems - sourceValues.Count();
            if (nbItemsToRemove <= 0)
            {
                return;
            }

            this.sourceValues.RemoveRange(0, nbItemsToRemove);
            this.values.RemoveRange(0, nbItemsToRemove);
        }

        private static decimal GetTrueRange(Candle current, Candle previous)
        {
            return (new[] { current.FullRange, Math.Abs(current.High - previous.Close), Math.Abs(current.Low - previous.Close)}).Max();
        }
    }
}