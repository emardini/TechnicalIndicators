namespace BrokerAdapter.BackTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BrokerAdapter.Oanda;

    using TechnicalIndicators;

    public class HistoricalBackTestAdapter : OandaAdapter
    {
        private const string OrderSideBuy = "buy";

        private const string OrderSideSell = "sell";

        private decimal balancePips;
        #region Fields

        private readonly List<Candle> historicalCandles;

        private readonly double periodInMinutes;

        private readonly Random rateGenerator = new Random(DateTime.UtcNow.DayOfYear);

        private readonly DateTime startDate;

        private readonly int startIndex;

        private readonly int ticksInPeriod;

        private bool hasOrder;

        private int nbOfCalls;
        private Trade currentTrade;
        private Rate CurrentRate;

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
            this.periodInMinutes = (secondCandle.Timestamp - firstCandle.Timestamp).TotalMinutes;
        }

        #endregion

        #region Public Methods and Operators

        public override void CloseTrade(int accountId, long tradeId)
        {
            var gainLoss = 0m;
            if (this.currentTrade.Side == OrderSideBuy)
            {
                    gainLoss = CurrentRate.Bid - currentTrade.Price;
                    Console.WriteLine("Stop loss triggered=>Gain/Loss={0}", gainLoss);
                    currentTrade = null;
                    balancePips += gainLoss;
                    Console.WriteLine("Balance = {0}", balancePips);
            }
            else
            {
                    hasOrder = false;
                    gainLoss = -CurrentRate.Ask + currentTrade.Price;
                    Console.WriteLine("Stop loss triggered=>Gain/Loss={0}", gainLoss);
                    currentTrade = null;
                    balancePips += gainLoss;
                    Console.WriteLine("Balance = {0}", balancePips);
            }
        }

        public override Trade GetOpenTrade(int accountId)
        {
            return this.currentTrade;
        }

        public override Candle GetLastCandle(string instrument, int periodInMinutes, DateTime? endDateTime = null)
        {
            return this.GetCurrentCandle(this.nbOfCalls);
        }

        public override IEnumerable<Candle> GetLastCandles(string instrument, int periodInMinutes, int nbOfCandles, DateTime? endDateTime = null)
        {
            var result = endDateTime.HasValue
                ? this.historicalCandles.Where(x => x.Timestamp <= endDateTime).OrderByDescending(x => x.Timestamp).Take(nbOfCandles)
                : this.historicalCandles.Where(x => x.Timestamp <= this.startDate).OrderByDescending(x => x.Timestamp).Take(nbOfCandles);

            return result.OrderBy(x => x.Timestamp);
        }

        public override Rate GetRate(string instrument)
        {
            var currentCandle = this.GetCurrentCandle(this.nbOfCalls);

            var rateValue = currentCandle.FullRange * (decimal)this.rateGenerator.NextDouble() + currentCandle.Low;

            var minuteFraction = (double)(this.nbOfCalls % this.ticksInPeriod) / this.ticksInPeriod;

            this.nbOfCalls++;

            this.CurrentRate = new Rate
            {
                Ask = rateValue,
                Bid = rateValue,
                Instrument = instrument,
                Time = currentCandle.Timestamp.AddMinutes(this.periodInMinutes * minuteFraction)
            };

            if (this.currentTrade == null)
            {
                return this.CurrentRate;
            }

            if (this.currentTrade.Side == OrderSideBuy)
            {
                var newTrailingAmount = CurrentRate.Bid - this.currentTrade.TrailingStop * 0.0001m;
                this.currentTrade.TrailingAmount = newTrailingAmount > this.currentTrade.TrailingAmount ? newTrailingAmount : this.currentTrade.TrailingAmount;
            }
            else
            {
                var newTrailingAmount = CurrentRate.Ask + this.currentTrade.TrailingStop * 0.0001m;
                this.currentTrade.TrailingAmount = newTrailingAmount < this.currentTrade.TrailingAmount ? newTrailingAmount : this.currentTrade.TrailingAmount;
            }

            var gainLoss = 0m;
            if (this.currentTrade.Side == OrderSideBuy)
            {
                if (CurrentRate.Bid < currentTrade.TrailingAmount)
                {
                    hasOrder = false;
                    gainLoss = CurrentRate.Bid - currentTrade.Price;
                    Console.WriteLine("Stop loss triggered=>Gain/Loss={0}", gainLoss);
                    currentTrade = null;
                    balancePips += gainLoss;
                    Console.WriteLine("Balance = {0}", balancePips);
                }
            }
            else
            {
                if (CurrentRate.Ask > currentTrade.TrailingAmount)
                {
                    hasOrder = false;
                    gainLoss = -CurrentRate.Ask + currentTrade.Price;
                    Console.WriteLine("Stop loss triggered=>Gain/Loss={0}", gainLoss);
                    currentTrade = null;
                    balancePips += gainLoss;
                    Console.WriteLine("Balance = {0}", balancePips);
                }
            }

            return CurrentRate;
        }

        public override bool HasOpenOrder(int accountId)
        {
            return this.hasOrder;
        }

        public override bool IsInstrumentHalted(string instrument)
        {
            return this.GetLastCandle(instrument, 1) == null;
        }

        public override void PlaceOrder(Order order)
        {
            this.hasOrder = true;
            var price = order.Side == OrderSideBuy ? CurrentRate.Ask : CurrentRate.Bid;
            var trailingSize = order.TrailingStop * 0.0001m;
            var trailingAmount = order.Side == OrderSideBuy ? price - trailingSize : price + trailingSize;
            this.currentTrade = new Trade { Side = order.Side, TrailingAmount = trailingAmount, Price = price, TrailingStop = order.TrailingStop, StopLoss = order.StopLoss};
            Console.WriteLine("Order placed ...");
        }

        public override bool HasOpenTrade(int accountId)
        {
            return hasOrder;
        }

        public void Reset()
        {
            this.nbOfCalls = 0;
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