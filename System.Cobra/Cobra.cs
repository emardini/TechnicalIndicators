namespace System.Cobra
{
    using System.Collections.Generic;
    using System.Linq;

    using TechnicalIndicators;

    public class Cobra
    {
        #region Constants

        private const int AdxTrendLevel = 22;

        #endregion

        #region Fields

        private readonly Adx adx;

        private readonly List<Candle> candles;

        private readonly IDateProvider dateProvider;

        private readonly Ema fastEmaHigh;

        private readonly Ema fastEmaLow;

        private readonly string instrument;

        public int PeriodInMinutes { get; private set; }

        private readonly Sma slowSmaHigh;

        private readonly Sma slowSmaLow;

        #endregion

        #region Constructors and Destructors

        public Cobra(Adx adx, IEnumerable<Candle> initialCandles, Ema fastEmaHigh, Ema fastEmaLow, Sma slowSmaHigh, Sma slowSmaLow, string instrument, int periodInMinutes)
            : this(adx, initialCandles, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, new SimpleDateProvider(), instrument, periodInMinutes)
        {
        }

        public Cobra(Adx adx,
            IEnumerable<Candle> initialCandles,
            Ema fastEmaHigh,
            Ema fastEmaLow,
            Sma slowSmaHigh,
            Sma slowSmaLow,
            IDateProvider dateProvider,
            string instrument, 
            int periodInMinutes)
        {
            if (adx == null)
            {
                throw new ArgumentNullException("adx");
            }
            if (initialCandles == null)
            {
                throw new ArgumentNullException("initialCandles");
            }
            if (fastEmaHigh == null)
            {
                throw new ArgumentNullException("fastEmaHigh");
            }
            if (fastEmaLow == null)
            {
                throw new ArgumentNullException("fastEmaLow");
            }
            if (slowSmaHigh == null)
            {
                throw new ArgumentNullException("slowSmaHigh");
            }
            if (slowSmaLow == null)
            {
                throw new ArgumentNullException("slowSmaLow");
            }
            if (dateProvider == null)
            {
                throw new ArgumentNullException("dateProvider");
            }
            if (string.IsNullOrWhiteSpace(instrument))
            {
                throw new ArgumentNullException("instrument");
            }

            this.adx = adx;
            this.candles = initialCandles.ToList();
            this.fastEmaHigh = fastEmaHigh;
            this.fastEmaLow = fastEmaLow;
            this.slowSmaHigh = slowSmaHigh;
            this.slowSmaLow = slowSmaLow;
            this.dateProvider = dateProvider;
            this.instrument = instrument;
            this.PeriodInMinutes = periodInMinutes;

            this.Id = Guid.NewGuid().ToString();
        }

        #endregion

        #region Public Properties

        public string Id { get; private set; }

        public string Instrument
        {
            get { return this.instrument; }
        }

        public Rate CurrentRate { get; private set; }

        #endregion

        #region Public Methods and Operators

        public static bool ConfirmPreviousCandleForAsk(Candle previousCandle, Candle currentCandle)
        {
            if (previousCandle == null)
            {
                return false;
            }

            if (currentCandle == null)
            {
                return false;
            }

            if (!previousCandle.IsUp)
            {
                return false;
            }

            return currentCandle.Close > previousCandle.Close;
        }

        public void AddCandle(Candle newCandle)
        {
            this.candles.Add(newCandle);
            this.adx.Add(newCandle);
            this.fastEmaHigh.Add(newCandle.High);
            this.fastEmaLow.Add(newCandle.Low);
            this.slowSmaHigh.Add(newCandle.High);
            this.slowSmaLow.Add(newCandle.Low);
        }

        //Return structure containig reason why cannot go long to improve testability
        public bool CanGoLong(Rate rate)
        {
            if (this.IsBannedDay())
            {
                return false;
            }

            var slowSmaHighValue = this.slowSmaHigh.Values.FirstOrDefault();
            var fastEmaHighValue = this.fastEmaHigh.Values.FirstOrDefault();

            if (fastEmaHighValue < slowSmaHighValue)
            {
                return false;
            }

            if (fastEmaHighValue > rate.Ask)
            {
                return false;
            }

            var currentCandle = this.candles.LastOrDefault();
            if (currentCandle == null)
            {
                return false;
            }

            if (!currentCandle.IsUp)
            {
                return false;
            }

            if (fastEmaHighValue > currentCandle.Open)
            {
                return false;
            }

            var previousCandle = this.candles.TakeLast(2).Skip(1).FirstOrDefault();
            if (!ConfirmPreviousCandleForAsk(previousCandle, currentCandle))
            {
                previousCandle = this.candles.TakeLast(3).Skip(2)
                    .FirstOrDefault();
                if (!ConfirmPreviousCandleForAsk(previousCandle, currentCandle))
                {
                    return false;
                }
            }

            if (currentCandle.IsReversal(GetThreshold(rate.Instrument)))
            {
                return false;
            }

            var currentAdxValue = this.adx.Values.FirstOrDefault();
            if (currentAdxValue < AdxTrendLevel)
            {
                return false;
            }

            var previousAdxValue = this.adx.Values.Skip(1)
                .Take(1)
                .FirstOrDefault();

            return currentAdxValue > previousAdxValue;
        }

        /// <summary>
        ///     Should happen every minute because the check needs to be frequent,
        ///     but the candles are queried in the predefined timeframe
        /// </summary>
        public void CheckRate(Rate newRate)
        {
            this.CurrentRate = newRate;

            //Check indicators have enough data

            if (this.HasOpenOrder())
            {
                if (this.ShouldCloseOrder())
                {
                    this.CloseOrder();
                }
            }

            if (this.CanGoLong(newRate))
            {
                this.PlaceLongOrder();
                return;
            }

            if (this.CanGoShort())
            {
                this.PlaceShortOrder();
            }
        }

        public bool IsBannedDay()
        {
            var currentDate = this.dateProvider.GetCurrentUtcDate();
            return currentDate.DayOfWeek == DayOfWeek.Friday
                   || currentDate.DayOfWeek == DayOfWeek.Saturday
                   || currentDate.DayOfWeek == DayOfWeek.Sunday;
        }

        #endregion

        #region Methods

        private static Threshold GetThreshold(string instrument)
        {
            switch (instrument)
            {
                case "EURUSD":
                    return new Threshold { Body = 0.1m, Delta = 0.0003m };
                default:
                    return new Threshold { Body = 0.1m, Delta = 0.0001m };
            }
        }

        private bool CanGoShort()
        {
            return false;
        }

        private void CloseOrder()
        {
        }

        private bool HasOpenOrder()
        {
            return false;
        }

        private void PlaceLongOrder()
        {
        }

        private void PlaceShortOrder()
        {
        }

        private bool ShouldCloseOrder()
        {
            return false;
        }

        #endregion
    }
}