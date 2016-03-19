﻿namespace System.Cobra
{
    using System.Collections.Generic;
    using System.Linq;

    using TechnicalIndicators;

    public class Cobra
    {
        #region Constants

        private const int AdxTrendLevel = 22;

        private const string OrderSideBuy = "buy";

        private const string OrderSideSell = "sell";

        private const string OrderTypeMarket = "market";

        private const int Slippage = 5;

        #endregion

        #region Fields

        private readonly Adx adx;

        private readonly List<Candle> candles;

        private readonly IDateProvider dateProvider;

        private readonly Ema fastEmaHigh;

        private readonly Ema fastEmaLow;

        private readonly string instrument;

        private readonly Sma slowSmaHigh;

        private readonly Sma slowSmaLow;

        private readonly ITradingAdapter tradingAdapter;

        #endregion

        #region Constructors and Destructors

        public Cobra(Adx adx,
            IEnumerable<Candle> initialCandles,
            Ema fastEmaHigh,
            Ema fastEmaLow,
            Sma slowSmaHigh,
            Sma slowSmaLow,
            IDateProvider dateProvider,
            string instrument,
            int periodInMinutes,
            ITradingAdapter tradingAdapter,
            int accountId)
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
            if (tradingAdapter == null)
            {
                throw new ArgumentNullException("tradingAdapter");
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
            this.tradingAdapter = tradingAdapter;
            this.AccountId = accountId;
            this.PeriodInMinutes = periodInMinutes;

            this.Id = Guid.NewGuid().ToString();
        }

        #endregion

        #region Public Properties

        public int AccountId { get; private set; }

        public Rate CurrentRate { get; private set; }

        public string Id { get; private set; }

        public string Instrument
        {
            get { return this.instrument; }
        }

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
            if (this.tradingAdapter.HasOpenOrder(this.AccountId))
            {
                return;
            }

            if (this.tradingAdapter.HasOpenTrade(this.AccountId))
            {
                var currentTrade = this.tradingAdapter.GetOpenTrade(this.AccountId);
                if (this.ShouldCloseTrade(currentTrade))
                {
                    this.tradingAdapter.CloseTrade(this.AccountId, currentTrade.Id);
                }
                else if (this.ShouldModifyTrade(currentTrade))
                {
                    var updatedTrade = new Trade { Id = currentTrade.Id, StopLoss = currentTrade.TrailingAmount, TrailingStop = 0, TakeProfit = 0 };
                    this.tradingAdapter.UpdateTrade(updatedTrade);
                    return;
                }
                else
                {
                    return;
                }
            }

            if (this.CanGoLong(newRate))
            {
                this.PlaceOrder(OrderSideBuy);
                return;
            }

            if (this.CanGoShort(newRate))
            {
                this.PlaceOrder(OrderSideSell);
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
                case "EURUSD":
                    return new Threshold { Body = 0.1m, Delta = 0.0003m };
                default:
                    return new Threshold { Body = 0.1m, Delta = 0.0001m };
            }
        }

        private int CalculatePositionSize()
        {
            //TODO: Use account balance and Kelly Criterior to calculate position size
            return (int)(3000.00m * 0.02m);
        }

        private decimal CalculateStopLossDistance()
        {
            //TODO: Calculate stop loss based on indicators 
            return 25;
        }

        private bool CanGoShort(Rate rate)
        {
            if (this.IsBannedDay())
            {
                return false;
            }

            var slowSmaLowValue = this.slowSmaLow.Values.FirstOrDefault();
            var fastEmaLowValue = this.fastEmaLow.Values.FirstOrDefault();

            if (slowSmaLowValue < rate.Ask)
            {
                return false;
            }

            if (fastEmaLowValue < rate.Ask)
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

        private void PlaceOrder(string side)
        {
            var positionSizeInUnits = this.CalculatePositionSize();
            var stopLossDistance = this.CalculateStopLossDistance();
            //TODO: Decide if to user lower-upper bounds or just market order and assume the slippage
            this.tradingAdapter.PlaceOrder(new Order
            {
                Instrument = this.instrument,
                Units = positionSizeInUnits,
                Side = side,
                OrderType = OrderTypeMarket,
                TrailingStop = stopLossDistance,
                AcountId = AccountId
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
            decimal? spread = (CurrentRate.Ask - CurrentRate.Bid)/2;
            switch (currentTrade.Side)
            {
                case OrderSideBuy:                    
                    return currentTrade.TrailingAmount >= currentTrade.Price + spread + Slippage;
                default:
                    return currentTrade.TrailingAmount <= currentTrade.Price - spread - Slippage;
            }
        }

        #endregion
    }
}