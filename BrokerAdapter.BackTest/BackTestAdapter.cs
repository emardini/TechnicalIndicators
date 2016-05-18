namespace BrokerAdapter.BackTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TechnicalIndicators;

    public class BackTestAdapter : IRateProvider, ITradingAdapter
    {
        #region Constants

        private const int CandlePeriod = 10;

        private const decimal DolarByPip = 0.0001m;

        private const int MaxCandleRange = 40;

        private const int MaxNbOfDownCandles = 50;

        private const int MaxNbOfUpCandles = 300;

        #endregion

        #region Fields

        private readonly IEnumerable<Candle> candles;

        private readonly Random randomRateGenerator = new Random();

        private int currentCandleIndex;

        private bool hasOpenOrder;

        private bool hasOpenTrade;

        #endregion

        #region Constructors and Destructors

        public BackTestAdapter()
        {
            this.candles = this.GetCandles();
            this.currentCandleIndex = 0;
        }

        #endregion

        #region Public Methods and Operators

        public void CloseTrade(int accountId, long tradeId)
        {
            this.hasOpenOrder = false;
            this.hasOpenTrade = false;
        }

        public AccountInformation GetAccountInformation(int accountId)
        {
            return new AccountInformation { Balance = "10000" };
        }

        public Candle GetLastCandle(string instrument, int periodInMinutes, DateTime? endDateTime = null)
        {
            return this.GetLastCandles(instrument, periodInMinutes, 1, endDateTime).FirstOrDefault();
        }

        public IEnumerable<Candle> GetLastCandles(string instrument, int periodInMinutes, int nbOfCandles, DateTime? endDateTime = null)
        {
            var sortedCandles = this.candles.OrderBy(x => x.Timestamp);
            var filteredCandles = endDateTime.HasValue ? sortedCandles.Where(x => x.Timestamp <= endDateTime) : sortedCandles;
            return filteredCandles.TakeLast(nbOfCandles);
        }

        public Trade GetOpenTrade(int accountId)
        {
            return new Trade();
        }

        public Rate GetRate(string instrument)
        {
            const int TotalNbOfCandles = MaxNbOfUpCandles + MaxNbOfDownCandles;
            var nextCandleIndex = this.currentCandleIndex++;
            if (nextCandleIndex >= TotalNbOfCandles)
            {
                nextCandleIndex = 0;
            }

            var nextCandle = this.candles.Skip(nextCandleIndex).Take(1).FirstOrDefault();

            var rateValue = nextCandle.High;
            var timestamp =
                nextCandle.Timestamp.AddMinutes(this.randomRateGenerator.Next(1, CandlePeriod - 1)).AddSeconds(this.randomRateGenerator.Next(1, 59));

            return new Rate { Ask = rateValue, Bid = rateValue, Instrument = instrument, Time = timestamp };
        }

        public bool HasOpenOrder(int accountId)
        {
            return this.hasOpenOrder;
        }

        public bool HasOpenTrade(int accountId)
        {
            return this.hasOpenTrade;
        }

        public bool IsInstrumentHalted(string instrument)
        {
            return false;
        }

        public void PlaceOrder(Order order)
        {
            this.hasOpenOrder = true;
            this.hasOpenTrade = true;
        }

        public void Reset()
        {
            this.currentCandleIndex = 0;
        }

        public void UpdateTrade(Trade updatedTrade)
        {
        }

        #endregion

        #region Methods

        private IEnumerable<Candle> GetCandles()
        {
            var startDate = DateTime.Now.AddMinutes(-MaxNbOfUpCandles * CandlePeriod);
            var tenthsOfMinutes = (startDate.Minute / CandlePeriod);
            startDate =
                new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, tenthsOfMinutes * CandlePeriod, 0).AddMinutes(
                    -MaxNbOfUpCandles);
            Candle lastCandle = null;
            for (var i = 0; i < MaxNbOfUpCandles; i++)
            {
                if (lastCandle == null)
                {
                    lastCandle = new Candle(1.3000m - 0.0015m, 1.3000m + 0.0020m, 1.3000m - 0.0020m, 1.3000m + 0.0015m, startDate);
                    yield return lastCandle;
                }

                var upPips = this.randomRateGenerator.Next(10, 20) / 10000m;
                lastCandle = new Candle(lastCandle.Close,
                    lastCandle.Close + 0.0020m + upPips,
                    lastCandle.Close - 0.001m,
                    lastCandle.Close + 0.0015m + upPips,
                    startDate);
                yield return lastCandle;

                startDate = startDate.AddMinutes(CandlePeriod);
            }

            for (var i = 0; i < MaxNbOfDownCandles; i++)
            {
                if (lastCandle == null)
                {
                    lastCandle = new Candle(1.3000m - 0.0015m, 1.3000m + 0.0020m, 1.3000m - 0.0020m, 1.3000m + 0.0015m, startDate);
                    yield return lastCandle;
                }

                var downPips = this.randomRateGenerator.Next(10, 20) / 10000m;
                lastCandle = new Candle(lastCandle.Close,
                    lastCandle.Close + 0.0005m,
                    lastCandle.Close - 0.020m - downPips,
                    lastCandle.Close - 0.0015m - downPips,
                    startDate);
                yield return lastCandle;
            }
        }

        #endregion
    }
}