namespace System.Cobra
{
    using System.Collections.Generic;
    using System.Linq;

    using TechnicalIndicators;

    public class Cobra
    {
        #region Constants

        private const int AdxTrendLevel = 22;

        private const decimal BaseRiskPercentage = 0.02m;

        private const decimal DolarsByPip = 0.0001m;

        private const int MaxRateStaleTime = 2;

        private const int MinNbOfCandles = 72;

        private const string OrderSideBuy = "buy";

        private const string OrderSideSell = "sell";

        private const string OrderTypeMarket = "market";

        private const int Slippage = 3;

        #endregion

        #region Fields

        private readonly Adx adx;

        private readonly List<Candle> candles;

        private readonly IDateProvider dateProvider;

        private readonly Ema fastEmaHigh;

        private readonly Ema fastEmaLow;

        private readonly bool isBackTest;

        private readonly IRateProvider rateProvider;

        private readonly Sma slowSmaHigh;

        private readonly Sma slowSmaLow;

        private readonly ITradingAdapter tradingAdapter;

        #endregion

        #region Constructors and Destructors

        public Cobra(Adx adx,
            Ema fastEmaHigh,
            Ema fastEmaLow,
            Sma slowSmaHigh,
            Sma slowSmaLow,
            IDateProvider dateProvider,
            string instrument,
            int periodInMinutes,
            ITradingAdapter tradingAdapter,
            IRateProvider rateProvider,
            int accountId,
            bool isBackTest = false)
        {
            if (adx == null)
            {
                throw new ArgumentNullException("adx");
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
            if (tradingAdapter == null)
            {
                throw new ArgumentNullException("tradingAdapter");
            }
            if (rateProvider == null)
            {
                throw new ArgumentNullException("rateProvider");
            }
            if (string.IsNullOrWhiteSpace(instrument))
            {
                throw new ArgumentNullException("instrument");
            }

            this.adx = adx;
            this.fastEmaHigh = fastEmaHigh;
            this.fastEmaLow = fastEmaLow;
            this.slowSmaHigh = slowSmaHigh;
            this.slowSmaLow = slowSmaLow;
            this.dateProvider = dateProvider;
            this.Instrument = instrument;
            this.tradingAdapter = tradingAdapter;
            this.rateProvider = rateProvider;
            this.isBackTest = isBackTest;
            this.AccountId = accountId;
            this.PeriodInMinutes = periodInMinutes;

            var initialCandles = rateProvider.GetLastCandles(instrument, periodInMinutes, MinNbOfCandles).ToList();
            this.candles = new List<Candle>();
            try
            {
                this.AddCandles(initialCandles);
            }
            catch (Exception)
            {
                //Log
            }

            this.Id = Guid.NewGuid().ToString();
        }

        #endregion

        #region Public Properties

        public int AccountId { get; private set; }

        public Rate CurrentRate { get; private set; }

        public string Id { get; private set; }

        public string Instrument { get; private set; }

        public int PeriodInMinutes { get; private set; }

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
            if (this.candles.Any(x => x.Timestamp == newCandle.Timestamp))
            {
                return;
            }

            var lastCandle = this.candles.LastOrDefault();
            if (lastCandle != null)
            {
                if ((newCandle.Timestamp - lastCandle.Timestamp).Minutes != this.PeriodInMinutes)
                {
                    throw new Exception("The new candle does not follow the sequence");
                }
            }

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

            var slowSmaHighValue = this.slowSmaHigh.Values.LastOrDefault();
            var fastEmaHighValue = this.fastEmaHigh.Values.LastOrDefault();

            if (slowSmaHighValue > rate.Ask)
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

            var currentAdxValue = this.adx.Values.LastOrDefault() * 100m;
            if (currentAdxValue < AdxTrendLevel)
            {
                return false;
            }

            var previousAdxValue = this.adx.Values.TakeLast(2).Skip(1).FirstOrDefault() * 100m;

            return currentAdxValue > previousAdxValue;
        }

        /// <summary>
        ///     Should happen every minute because the check needs to be frequent,
        ///     but the candles are queried in the predefined timeframe
        /// </summary>
        public void CheckRate()
        {
            if (this.rateProvider.IsInstrumentHalted(this.Instrument))
            {
                return;
            }

            var newRate = this.rateProvider.GetRate(this.Instrument);
            if (this.CurrentRate != null && newRate.Time <= this.CurrentRate.Time)
            {
                return;
            }

            if (!this.isBackTest)
            {
                var systemTimeDiff = this.dateProvider.GetCurrentUtcDate() - newRate.Time;
                if (systemTimeDiff.Minutes >= MaxRateStaleTime)
                {
                    //Meaning rate reading is stale
                    return;
                }
            }

            this.CurrentRate = newRate;

            var nbOfRequiredCandles = 1;
            var lastCandle = this.candles.OrderByDescending(x => x.Timestamp).FirstOrDefault();
            if (lastCandle != null)
            {
                nbOfRequiredCandles = (this.CurrentRate.Time - lastCandle.Timestamp).Minutes / this.PeriodInMinutes;
            }

            if (nbOfRequiredCandles > 0)
            {
                var requiredCandles = this.rateProvider.GetLastCandles(this.Instrument,
                    this.PeriodInMinutes,
                    nbOfRequiredCandles,
                    this.CurrentRate.Time).ToList();
                if (requiredCandles.Count() < nbOfRequiredCandles)
                {
                    return;
                }
                var hasError = false;
                try
                {
                    this.AddCandles(requiredCandles);
                }
                catch (Exception)
                {
                    //Log
                    hasError = true;
                }

                if (hasError)
                {
                    return;
                }
            }

            if (!this.ValidateIndicatorsState())
            {
                return;
            }

            if (this.tradingAdapter.HasOpenTrade(this.AccountId))
            {
                var currentTrade = this.tradingAdapter.GetOpenTrade(this.AccountId);
                //Not sure if doing this or just keep the trailing stop
                if (this.ShouldCloseTrade(currentTrade))
                {
                    Console.WriteLine("Closing trade");
                    this.tradingAdapter.CloseTrade(this.AccountId, currentTrade.Id);
                }
                else if (this.ShouldModifyTrade(currentTrade))
                {
                    Console.WriteLine("Break even");
                    var updatedTrade = new Trade { Id = currentTrade.Id, StopLoss = currentTrade.TrailingAmount, TrailingStop = 0, TakeProfit = 0 };
                    this.tradingAdapter.UpdateTrade(updatedTrade);
                    return;
                }
                else
                {
                    return;
                }
            }

            if (this.tradingAdapter.HasOpenOrder(this.AccountId))
            {
                return;
            }

            if (this.CanGoLong(this.CurrentRate))
            {
                this.PlaceOrder(OrderSideBuy, this.CurrentRate);
                return;
            }

            if (this.CanGoShort(this.CurrentRate))
            {
                this.PlaceOrder(OrderSideSell, this.CurrentRate);
            }
        }

        public bool IsBannedDay()
        {
            var currentDate = this.dateProvider.GetCurrentDate();
            return currentDate.DayOfWeek == DayOfWeek.Friday
                   || currentDate.DayOfWeek == DayOfWeek.Saturday
                   || currentDate.DayOfWeek == DayOfWeek.Sunday;
        }

        #endregion

        #region Methods

        private static bool ConfirmPreviousCandleForBid(Candle previousCandle, Candle currentCandle)
        {
            if (previousCandle == null)
            {
                return false;
            }

            if (currentCandle == null)
            {
                return false;
            }

            if (!previousCandle.IsDown)
            {
                return false;
            }

            return currentCandle.Close < previousCandle.Close;
        }

        private static Threshold GetThreshold(string instrument)
        {
            switch (instrument)
            {
                case "EUR_USD":
                    return new Threshold { Body = 0.1m, Delta = 0.0003m };
                default:
                    return new Threshold { Body = 0.1m, Delta = 0.0001m };
            }
        }

        private void AddCandles(IEnumerable<Candle> initialCandles)
        {
            var sortedCandles = initialCandles.OrderBy(x => x.Timestamp).ToList();
            if (!sortedCandles.Any())
            {
                return;
            }

            var previousCandle = sortedCandles.First();
            foreach (var candle in sortedCandles.Skip(1))
            {
                if ((candle.Timestamp - previousCandle.Timestamp).Minutes != this.PeriodInMinutes)
                {
                    throw new Exception("The list of candles do not follow the sequence");
                }
                previousCandle = candle;
            }

            foreach (var candle in sortedCandles)
            {
                this.AddCandle(candle);
            }
        }

        private int CalculatePositionSize(decimal stopLoss)
        {
            var accountInformation = this.tradingAdapter.GetAccountInformation(this.AccountId);
            var maxRiskAmount = accountInformation.Balance.SafeParseDecimal().GetValueOrDefault() * BaseRiskPercentage;

            var positionSize = (maxRiskAmount / stopLoss) / DolarsByPip;
            //TODO: Use account balance and Kelly Criterior to calculate position size
            return (int)positionSize;
        }

        private decimal CalculateStopLossDistance(string side)
        {
            if (side == OrderSideBuy)
            {
                var lowLimit = this.fastEmaLow.Values.LastOrDefault();
                return Math.Ceiling((this.CurrentRate.Ask - lowLimit) / DolarsByPip);
            }

            var highLimit = this.fastEmaHigh.Values.LastOrDefault();
            return Math.Ceiling((highLimit - this.CurrentRate.Bid) / DolarsByPip);
        }

        private bool CanGoShort(Rate rate)
        {
            if (this.IsBannedDay())
            {
                return false;
            }

            var slowSmaLowValue = this.slowSmaLow.Values.LastOrDefault();
            var fastEmaLowValue = this.fastEmaLow.Values.LastOrDefault();

            if (slowSmaLowValue < rate.Bid)
            {
                return false;
            }

            if (fastEmaLowValue < rate.Bid)
            {
                return false;
            }

            var currentCandle = this.candles.LastOrDefault();
            if (currentCandle == null)
            {
                return false;
            }

            if (!currentCandle.IsDown)
            {
                return false;
            }

            if (fastEmaLowValue < currentCandle.Open)
            {
                return false;
            }

            var previousCandle = this.candles.TakeLast(2).Skip(1).FirstOrDefault();
            if (!ConfirmPreviousCandleForBid(previousCandle, currentCandle))
            {
                previousCandle = this.candles.TakeLast(3).Skip(2)
                    .FirstOrDefault();
                if (!ConfirmPreviousCandleForBid(previousCandle, currentCandle))
                {
                    return false;
                }
            }

            if (currentCandle.IsReversal(GetThreshold(rate.Instrument)))
            {
                return false;
            }

            var currentAdxValue = this.adx.Values.LastOrDefault() * 100;
            if (currentAdxValue < AdxTrendLevel)
            {
                return false;
            }

            var previousAdxValue = this.adx.Values.TakeLast(2).Skip(1).FirstOrDefault() * 100m;

            return currentAdxValue > previousAdxValue;
        }

        private void PlaceOrder(string side, Rate rate)
        {
            var stopLossDistance = this.CalculateStopLossDistance(side);
            var positionSizeInUnits = this.CalculatePositionSize(stopLossDistance);
            //TODO: Decide if to user lower-upper bounds or just market order and assume the slippage
            this.tradingAdapter.PlaceOrder(new Order
            {
                Instrument = this.Instrument,
                Units = positionSizeInUnits,
                Side = side,
                OrderType = OrderTypeMarket,
                TrailingStop = stopLossDistance,
                AcountId = this.AccountId,
                Timestamp = rate.Time
            });
        }

        private bool ShouldCloseTrade(Trade currentTrade)
        {
            var currentCandle = this.candles.LastOrDefault();

            switch (currentTrade.Side)
            {
                case OrderSideBuy:
                {
                    var fastEmaLowValue = this.fastEmaLow.Values.FirstOrDefault();
                    return currentCandle.IsDown && currentCandle.Close < fastEmaLowValue;
                }
                case OrderSideSell:
                {
                    var fastEmaHighValue = this.fastEmaHigh.Values.FirstOrDefault();
                    return currentCandle.IsUp && currentCandle.Close < fastEmaHighValue;
                }
            }

            return true;
            //This should not happen, but in case it happens, it could be wise to close the trade or maybe send a notification                
        }

        private bool ShouldModifyTrade(Trade currentTrade)
        {
            if (currentTrade.StopLoss > 0)
            {
                return false;
            }

            decimal? spread = (this.CurrentRate.Ask - this.CurrentRate.Bid) / 2;
            switch (currentTrade.Side)
            {
                case OrderSideBuy:
                    return currentTrade.TrailingAmount >= currentTrade.Price + (spread + Slippage) * DolarsByPip;
                default:
                    return currentTrade.TrailingAmount <= currentTrade.Price - (spread - Slippage) * DolarsByPip;
            }
        }

        private bool ValidateIndicatorsState()
        {
            return
                this.candles.Count() >= MinNbOfCandles
                && (this.candles.Last().Timestamp - this.CurrentRate.Time).Minutes <= this.PeriodInMinutes;
        }

        #endregion
    }
}