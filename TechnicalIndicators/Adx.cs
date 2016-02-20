namespace TechnicalIndicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    //Use http://www.dinosaurtech.com/2007/average-directional-movement-index-adx-formula-in-c-2/
    //Or http://www.theforexguy.com/average-directional-index-indicator/
    public class Adx
    {
        #region Constants

        protected const int MaxNbOfItems = 5000;

        protected const int MaxPeriod = 200;

        #endregion

        #region Fields

        private readonly List<decimal> downDiAccum;

        private readonly List<decimal> downDmAccum;

        private readonly List<decimal> downDms;

        private readonly List<decimal> dxs;

        private readonly int period;

        private readonly List<Candle> sourceValues;

        private readonly List<decimal> trueRanges;

        private readonly List<decimal> trueRangesAccum;

        private readonly List<decimal> upDiAccum;

        private readonly List<decimal> upDmAccum;

        private readonly List<decimal> upDms;

        private readonly List<decimal> values;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Adx" /> class.
        ///     The Adx is implemented following the classical approach with simple moving average
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

            this.trueRangesAccum = new List<decimal>();
            this.upDmAccum = new List<decimal>();
            this.downDmAccum = new List<decimal>();
            this.upDiAccum = new List<decimal>();
            this.downDiAccum = new List<decimal>();

            this.dxs = new List<decimal>();
        }

        #endregion

        #region Public Properties

        public IEnumerable<decimal> DownDiAccum
        {
            get { return this.downDiAccum; }
        }

        public IEnumerable<decimal> DownDmAccum
        {
            get { return this.downDmAccum; }
        }

        public IEnumerable<decimal> DownDms
        {
            get { return this.downDms; }
        }
        public IEnumerable<decimal> Dxs
        {
            get { return this.dxs; }
        }

        public IEnumerable<Candle> SourceValues
        {
            get { return this.sourceValues; }
        }

        public IEnumerable<decimal> TrueRanges
        {
            get { return this.trueRanges; }
        }

        public IEnumerable<decimal> TrueRangesAccum
        {
            get { return this.trueRangesAccum; }
        }

        public IEnumerable<decimal> UpDiAccum
        {
            get { return this.upDiAccum; }
        }
        public IEnumerable<decimal> UpDmAccum
        {
            get { return this.upDmAccum; }
        }

        public IEnumerable<decimal> UpDms
        {
            get { return this.upDms; }
        }
        public IEnumerable<decimal> Values
        {
            get { return this.values; }
        }

        #endregion

        #region Public Methods and Operators

        public void Add(Candle value)
        {
            if (!this.sourceValues.Any())
            {
                this.sourceValues.Add(value);
                return;
            }

            var lastValue = this.sourceValues.LastOrDefault() ?? new Candle();

            var upMove = value.High - lastValue.High;
            var downMove = lastValue.Low - value.Low;

            var upDm = upMove > 0 && upMove > downMove ? upMove : 0;
            var downDm = downMove > 0 && downMove > upMove ? downMove : 0;

            var trueRange = GetTrueRange(value, lastValue);

            this.upDms.Add(upDm);
            this.downDms.Add(downDm);
            this.trueRanges.Add(trueRange);
            if (this.trueRanges.Count < this.period)
            {
                this.trueRangesAccum.Add(this.trueRanges.TakeLast(this.period).Sum());
                this.upDmAccum.Add(this.upDms.TakeLast(this.period).Sum());
                this.downDmAccum.Add(this.downDms.TakeLast(this.period).Sum());
            }
            else
            {
                //Just add the ups and downs with the same technique
                var previousTr = this.trueRangesAccum.Last();
                var currentTr = previousTr - (previousTr / this.period) + trueRange;
                this.trueRangesAccum.Add(currentTr);

                var previousUpDm = this.upDmAccum.Last();
                var currentUpDm = previousUpDm - (previousUpDm / this.period) + upDm;
                this.upDmAccum.Add(currentUpDm);

                var previousDownDm = this.downDmAccum.Last();
                var currentDownDm = previousDownDm - (previousDownDm / this.period) + downDm;
                this.downDmAccum.Add(currentDownDm);

                this.upDiAccum.Add(this.upDmAccum.Last() / this.trueRangesAccum.Last());
                this.downDiAccum.Add(this.downDmAccum.Last() / this.trueRangesAccum.Last());

                var delta = Math.Abs(this.upDiAccum.Last() - this.downDiAccum.Last());
                var divider = this.upDiAccum.Last() + this.downDiAccum.Last();
                var dx = divider > 0 ? delta / divider : 0;
                this.dxs.Add(dx);
            }

            if (this.dxs.Count == this.period)
            {
                this.values.Add(this.dxs.Average());
            }
            else if (this.dxs.Count > this.period)
            {
                var dxsAccum = (this.values.Last() * (this.period - 1) + this.dxs.Last()) / this.period;
                this.values.Add(dxsAccum);
            }

            this.sourceValues.Add(value);
        }

        #endregion

        #region Methods

        private static decimal GetTrueRange(Candle current, Candle previous)
        {
            return (new[] { current.FullRange, Math.Abs(current.High - previous.Close), Math.Abs(current.Low - previous.Close) }).Max();
        }

        #endregion
    }
}