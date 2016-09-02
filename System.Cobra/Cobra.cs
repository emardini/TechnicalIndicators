namespace System.Cobra
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Newtonsoft.Json;

    using TechnicalIndicators;

    /// <summary>
    /// Only works if the account currency is USD
    /// </summary>
    public class Cobra
    {
        #region Constants

        private const int AdxTrendLevel = 22;

        private const decimal BaseRiskPercentage = 0.02m;

        //private const decimal DolarsByPip = 0.0001m;

        private const int MaxRateStaleTime = 2;

        private const int MinNbOfCandles = 72;

        private const string OrderSideBuy = "buy";

        private const string OrderSideSell = "sell";

        private const string OrderTypeMarket = "market";

        private const int SlippagePips = 3;

        private const int MarginalGainPips = 3;

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
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
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

        public bool ConfirmPreviousCandleForAsk(Candle previousCandle, Candle currentCandle)
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
                if ((newCandle.Timestamp - lastCandle.Timestamp).Minutes % this.PeriodInMinutes > 0)
                {
                    throw new Exception(string.Format("The list of candles do not follow the sequence: {1} to {1}", lastCandle.Timestamp, newCandle.Timestamp));
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

            if (currentCandle.IsReversal(GetThreshold()))
            {
                return false;
            }

            var previousCandle = this.candles.TakeLast(2).Skip(1).FirstOrDefault();

            if (previousCandle.IsReversal(GetThreshold()))
            {
                return false;
            }

            if (!ConfirmPreviousCandleForAsk(previousCandle, currentCandle))
            {
                previousCandle = this.candles.TakeLast(3).Skip(2)
                    .FirstOrDefault();

                if (previousCandle.IsReversal(GetThreshold()))
                {
                    return false;
                }

                if (!ConfirmPreviousCandleForAsk(previousCandle, currentCandle))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Should happen every minute because the check needs to be frequent,
        ///     but the candles are queried in the predefined timeframe
        /// </summary>
        public void CheckRate()
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();

            if (this.rateProvider.IsInstrumentHalted(this.Instrument))
            {
                Trace.TraceInformation("Instrument {0} halted", this.Instrument);
                return;
            }

            if (this.tradingAdapter.HasOpenTrade(this.AccountId))
            {
                var currentTrade = this.tradingAdapter.GetOpenTrade(this.AccountId);
                //Not sure if doing this or just keep the trailing stop
                if (this.ShouldCloseTrade(currentTrade))
                {
                    Trace.TraceInformation("Closing trade");
                    this.tradingAdapter.CloseTrade(this.AccountId, currentTrade.Id);
                }
                else if (this.ShouldModifyTrade(currentTrade))
                {
                    Trace.TraceInformation("Break even");
                    var updatedTrade = new Trade { Id = currentTrade.Id, StopLoss = currentTrade.TrailingAmount, TrailingStop = 0, TakeProfit = 0 };
                    this.tradingAdapter.UpdateTrade(updatedTrade);
                    return;
                }
                else
                {
                    Trace.TraceInformation("Open trade");
                    return;
                }
            }

            if (this.tradingAdapter.HasOpenOrder(this.AccountId))
            {
                Trace.TraceInformation("Open order");
                return;
            }

            if (this.IsBannedDay())
            {
                Trace.TraceInformation("Non trading day");
                return;
            }

            var newRate = this.rateProvider.GetRate(this.Instrument);
            if (this.CurrentRate != null && newRate.Time <= this.CurrentRate.Time)
            {
                Trace.TraceInformation("No new rates");
                return;
            }

            Trace.TraceInformation("New rate : {0}", JsonConvert.SerializeObject(newRate));
            //TODO: Change logic to prioritize close/modify order in case of unstable conditions

            if (!this.isBackTest)
            {
                var systemTimeDiff = this.dateProvider.GetCurrentUtcDate() - newRate.Time.ToUniversalTime();
                if (systemTimeDiff.TotalMinutes >= MaxRateStaleTime)
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
                var timeDiffMinutesRaw = Math.Abs((this.CurrentRate.Time - lastCandle.Timestamp).TotalMinutes);
                var timeDiffMinutes = timeDiffMinutesRaw > int.MaxValue ? int.MaxValue : (int)timeDiffMinutesRaw;
                nbOfRequiredCandles = (timeDiffMinutes - this.PeriodInMinutes) / this.PeriodInMinutes;
                nbOfRequiredCandles = nbOfRequiredCandles < 0 ? 0 : nbOfRequiredCandles;
            }

            var nbOfcandles = this.candles.Count();
            if (nbOfRequiredCandles > 0)
            {
                var requiredCandles = this.rateProvider.GetLastCandles(this.Instrument,
                    this.PeriodInMinutes,
                    nbOfRequiredCandles).ToList();
                if (requiredCandles.Count() < nbOfRequiredCandles)
                {
                    Trace.TraceInformation("Not enough candles to check trading");
                    return;
                }

                try
                {
                    this.AddCandles(requiredCandles);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    return;
                }
            }

            if (nbOfcandles + nbOfRequiredCandles > this.candles.Count())
            {
                Trace.TraceInformation("System lagging behind in candles");
                return;
            }

            if (!this.ValidateIndicatorsState())
            {
                Trace.TraceInformation("Incomplete indicator values");
                return;
            }        

            var currentAdxValue = this.adx.Values.LastOrDefault() * 100;
            if (currentAdxValue < AdxTrendLevel)
            {
                Trace.TraceInformation("ADX ({0}) below threshold ({1})", currentAdxValue, AdxTrendLevel);
                return;
            }

            //TODO: Set history to calculate adx direction in app config
            //Calculating adx trend with more than one candle back will cause to lost many chances to enter and will enter late
            var previousAdxValue = this.adx.Values.TakeLast(2).Skip(1).FirstOrDefault() * 100m;

            if (currentAdxValue <= previousAdxValue)
            {
                Trace.TraceInformation("ADX not increasing {0} => {1}", previousAdxValue, currentAdxValue);
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

        private static Threshold GetThreshold()
        {
            return new Threshold { Body = 0.1m, Delta = 0.0003m };
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
                if ((candle.Timestamp - previousCandle.Timestamp).Minutes % this.PeriodInMinutes > 0)
                {
                    throw new Exception(string.Format("The list of candles do not follow the sequence: {1} to {1}", previousCandle.Timestamp, candle.Timestamp));
                }
                previousCandle = candle;
            }

            foreach (var candle in sortedCandles)
            {
                this.AddCandle(candle);
            }
        }

        private int CalculatePositionSize(decimal stopLoss, string side, Rate currentRate)
        {
            var accountInformation = this.tradingAdapter.GetAccountInformation(this.AccountId);
            //Account currency should be USD
            var balance = accountInformation.Balance.SafeParseDecimal().GetValueOrDefault();
            var maxRiskAmount = balance * BaseRiskPercentage;

            var pipFraction = GetPipFraction(currentRate.QuoteCurrency);

            var useRate = side == OrderSideBuy ? currentRate.Ask : currentRate.Bid;
            //Instead of hardcoding the account currency, better to read it from the api.
            var maxRiskAmountInQuote = currentRate.QuoteCurrency == "USD" ? maxRiskAmount : maxRiskAmount * useRate;
            var positionSize = (maxRiskAmountInQuote / stopLoss) / pipFraction;

            var accountMarginRate = accountInformation.MarginRate.SafeParseDecimal().GetValueOrDefault();
            var accountLeverage = 1m;
            if (accountMarginRate > 0)
            {
                accountLeverage = 1m / accountMarginRate;
            }

            var availablePositionSize = Math.Min(positionSize, balance * accountLeverage * useRate);

            //TODO: Use Kelly Criterior to calculate position size
            //TODO: Implement criterias for minimum stop loss condition

            return (int)availablePositionSize;
        }

        private static decimal GetPipFraction(string quoteCurrency)
        {
            return quoteCurrency == "JPY" ? 0.01M : 0.0001m;
        }

        private decimal CalculateStopLossDistanceInPips(string side, string quoteCurrency)
        {
            var pipFraction = GetPipFraction(quoteCurrency);
            if (side == OrderSideBuy)
            {
                var lowLimit = this.fastEmaLow.Values.LastOrDefault();
                return Math.Ceiling((this.CurrentRate.Ask - lowLimit) / pipFraction);
            }

            var highLimit = this.fastEmaHigh.Values.LastOrDefault();
            return Math.Abs((highLimit - this.CurrentRate.Bid) / pipFraction);
        }

        private bool CanGoShort(Rate rate)
        {
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

            if (currentCandle.IsReversal(GetThreshold()))
            {
                return false;
            }

            var previousCandle = this.candles.TakeLast(2).Skip(1).FirstOrDefault();

            if (previousCandle.IsReversal(GetThreshold()))
            {
                return false;
            }

            if (!ConfirmPreviousCandleForBid(previousCandle, currentCandle))
            {
                previousCandle = this.candles.TakeLast(3).Skip(2)
                    .FirstOrDefault();
                if (previousCandle.IsReversal(GetThreshold()))
                {
                    return false;
                }

                if (!ConfirmPreviousCandleForBid(previousCandle, currentCandle))
                {
                    return false;
                }
            }

            return true;
        }

        private void PlaceOrder(string side, Rate rate)
        {
            var stopLossDistance = this.CalculateStopLossDistanceInPips(side, rate.QuoteCurrency);
            var positionSizeInUnits = this.CalculatePositionSize(stopLossDistance, side, rate);
            //TODO: Decide if to user lower-upper bounds or just market order and assume the slippage

            var newOrder = new Order
            {
                Instrument = this.Instrument,
                Units = positionSizeInUnits,
                Side = side,
                OrderType = OrderTypeMarket,
                TrailingStop = stopLossDistance,
                AcountId = this.AccountId,
                Timestamp = rate.Time
            };
            this.tradingAdapter.PlaceOrder(newOrder);

            Trace.TraceInformation("Order placed :{0}", JsonConvert.SerializeObject(newOrder));
        }

        private bool ShouldCloseTrade(Trade currentTrade)
        {
            if (currentTrade.TrailingStop > 0)
            {
                return false;
            }

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

            var pipFraction = GetPipFraction(currentTrade.QuoteCurrency);
            decimal? spreadPips = (this.CurrentRate.Ask - this.CurrentRate.Bid) / (2 * pipFraction);
            var cushionDeltaPrice = (spreadPips + SlippagePips + MarginalGainPips) * pipFraction;
            switch (currentTrade.Side)
            {
                case OrderSideBuy:
                    return currentTrade.TrailingAmount >= currentTrade.Price + cushionDeltaPrice;
                default:
                    return currentTrade.TrailingAmount <= currentTrade.Price - cushionDeltaPrice;
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