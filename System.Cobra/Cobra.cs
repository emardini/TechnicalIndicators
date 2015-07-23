namespace System.Cobra
{
    using System.Collections.Generic;
    using System.Linq;

    using TechnicalIndicators;

    public class Cobra
    {
        private readonly Sma slowSmaLow;

        private readonly Sma slowSmaHigh;

        private readonly Ema fastEmaLow;

        private readonly Ema fastEmaHigh;

        private readonly Adx adx;

        private readonly Rate currentRate;

        private readonly List<Candle> candles;

        /// <summary>
        ///     Should happen every minute because the check needs to be frequent,
        ///     but the candles are queried in the predefined timeframe
        /// </summary>
        public void OnTick()
        {
            if (this.HasOpenOrder())
            {
                if (this.ShouldCloseOrder())
                {
                    this.CloseOrder();
                }
                return;
            }

            if (this.CanGoLong())
            {
                this.PlaceLongOrder();
                return;
            }

            if (this.CanGoShort())
            {
                this.PlaceShortOrder();
            }
        }

        private void PlaceShortOrder()
        {
        }

        private void PlaceLongOrder()
        {
        }

        private bool HasOpenOrder()
        {
            return false;
        }

        private void CloseOrder()
        {
        }

        private bool ShouldCloseOrder()
        {
            return false;
        }

        private bool CanGoShort()
        {
            return false;
        }

        private bool CanGoLong()
        {
            if (IsBannedDays())
            {
                return false;
            }

            var rate = this.currentRate.Ask;
            var slowSmaHighValue = this.slowSmaHigh.Values.FirstOrDefault();
            var fastEmaHighValue = this.fastEmaHigh.Values.FirstOrDefault();

            if (slowSmaHighValue >= fastEmaHighValue)
            {
                return false;
            }

            if (rate <= slowSmaHighValue || rate <= fastEmaHighValue)
            {
                return false;
            }

            var currentCandle = this.candles.FirstOrDefault();
            if (currentCandle == null)
            {
                return false;
            }

            if (!currentCandle.IsAskUp)
            {
                return false;
            }

            if (currentCandle.OpenAsk <= slowSmaHighValue || currentCandle.OpenAsk <= fastEmaHighValue)
            {
                return false;
            }

            if (currentCandle.CloseAsk <= slowSmaHighValue || currentCandle.CloseAsk <= fastEmaHighValue)
            {
                return false;
            }

            var previousCandle = this.candles.Skip(1)
                .Take(1)
                .FirstOrDefault();
            if (!ConfirmPreviousCandleForAsk(previousCandle, currentCandle))
            {
                previousCandle = this.candles.Skip(2)
                    .Take(1)
                    .FirstOrDefault();
                if (!ConfirmPreviousCandleForAsk(previousCandle, currentCandle))
                {
                    return false;
                }
            }

            if (currentCandle.IsAskReversal(GetThreshold(this.currentRate.Instrument)))
            {
                return false;
            }

            var currentAdxValue = this.adx.Values.FirstOrDefault();
            if (currentAdxValue < 22)
            {
                return false;
            }
            var previousAdxValue = this.adx.Values.Skip(1)
                .Take(1)
                .FirstOrDefault();
            if (currentAdxValue <= previousAdxValue)
            {
                return false;
            }

            return true;
        }

        private static bool IsBannedDays()
        {
            return DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday || DateTime.UtcNow.DayOfWeek == DayOfWeek.Saturday ||
                   DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday;
        }

        private static bool ConfirmPreviousCandleForAsk(Candle previousCandle, Candle currentCandle)
        {
            if (previousCandle == null)
            {
                return false;
            }
            if (!previousCandle.IsAskUp)
            {
                return false;
            }
            if (currentCandle.CloseAsk <= previousCandle.CloseAsk)
            {
                return false;
            }
            return true;
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
    }
}