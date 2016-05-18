namespace BrokerAdapter.BackTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BrokerAdapter.Oanda;

    using TechnicalIndicators;

    public class HistoricalBackTestAdapter : OandaAdapter
    {
        #region Fields

        private readonly List<Candle> historicalCandles;

        private readonly Random rateGenerator = new Random(DateTime.UtcNow.DayOfYear);

        private readonly DateTime startDate;

        private readonly int startIndex;

        private readonly int ticksInPeriod;

        private int nbOfCalls;

        private readonly double periodInMinutes;

        #endregion

        #region Constructors and Destructors

        public HistoricalBackTestAdapter(string accountUrl,
            string ratesUrl,
            string streamingRatesUrl,
            string streamingEventsUrl,
            string labsUrl,
            string token,
            int ticksInPeriod,
            IEnumerable<Candle> historicalCandles,
            DateTime startDate) : base(accountUrl, ratesUrl, streamingRatesUrl, streamingEventsUrl, labsUrl, token)
        {
            if (historicalCandles == null)
            {
                throw new ArgumentNullException("historicalCandles");
            }

            if (ticksInPeriod <= 0)
            {
                throw new ArgumentException("ticksInPeriod has to be greater than zero", "ticksInPeriod");
            }

            this.ticksInPeriod = ticksInPeriod;
            this.startDate = startDate;
            this.historicalCandles = historicalCandles.ToList();

            if (this.historicalCandles.Count() < 10)
            {
                throw new ArgumentException("historicalCandles has to be contain at least 10 elements", "historicalCandles");
            }

            var startCandle = this.historicalCandles.OrderByDescending(x => x.Timestamp).FirstOrDefault(x => x.Timestamp <= startDate);
            this.startIndex = this.historicalCandles.IndexOf(startCandle);

            var firstCandle = this.historicalCandles.FirstOrDefault();
            var secondCandle = this.historicalCandles.Skip(1).Take(1).FirstOrDefault();
            periodInMinutes = (secondCandle.Timestamp - firstCandle.Timestamp).TotalMinutes;
        }

        #endregion

        #region Public Methods and Operators

        public override void CloseTrade(int accountId, long tradeId)
        {
        }

        public override Candle GetLastCandle(string instrument, int periodInMinutes, DateTime? endDateTime = null)
        {
            return this.GetCurrentCandle(this.nbOfCalls);
        }

        public override IEnumerable<Candle> GetLastCandles(string instrument, int periodInMinutes, int nbOfCandles, DateTime? endDateTime = null)
        {
            var result =  endDateTime.HasValue
                ? this.historicalCandles.Where(x => x.Timestamp <= endDateTime).OrderBy(x => x.Timestamp).Take(nbOfCandles)
                : this.historicalCandles.Where(x => x.Timestamp <= this.startDate).OrderBy(x => x.Timestamp).Take(nbOfCandles);

            return result.OrderBy(x => x.Timestamp);
        }

        public override Rate GetRate(string instrument)
        {
            var currentCandle = this.GetCurrentCandle(this.nbOfCalls);

            var rateValue = currentCandle.FullRange * (decimal)this.rateGenerator.NextDouble() + currentCandle.Low;

            var minuteFraction = (double)(nbOfCalls % ticksInPeriod) / ticksInPeriod;

            this.nbOfCalls++;

            return new Rate { Ask = rateValue, Bid = rateValue, Instrument = instrument, Time = currentCandle.Timestamp.AddMinutes(periodInMinutes * minuteFraction) };
        }

        public override bool IsInstrumentHalted(string instrument)
        {
            return this.GetLastCandle(instrument, 1) == null;
        }

        public override void PlaceOrder(Order order)
        {
        }

        public override void UpdateTrade(Trade updatedTrade)
        {
        }

        #endregion

        #region Methods

        private Candle GetCurrentCandle(int calls)
        {
            return this.historicalCandles.ElementAtOrDefault(this.startIndex + calls / this.ticksInPeriod);
        }

        #endregion
    }
}