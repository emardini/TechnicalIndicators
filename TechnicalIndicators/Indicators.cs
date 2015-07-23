namespace TechnicalIndicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Indicators
    {
        public static IEnumerable<decimal> ADX(this IEnumerable<Candle> candles, int periods)
        {
            if (candles == null)
            {
                throw new ArgumentNullException("candles");
            }

            if (periods <= 0)
            {
                throw new ArgumentException("periods should be greater than zero (0)", "periods");
            }

            var linkedCandles = new LinkedList<Candle>(candles);

            if (periods >= linkedCandles.Count())
            {
                throw new ArgumentException("No enough candles for the given period", "periods");
            }

            var currentCandleNode = linkedCandles.First;
            var previousCandleNode = currentCandleNode.Previous;

            var period = 0;
            var dmPlusList = new List<decimal>();
            var dmMinusList = new List<decimal>();
            var trueRangeList = new List<decimal>();

            while (period < periods)
            {
                var currentCandle = currentCandleNode.Value;
                var previousCandle = previousCandleNode.Value;

                var deltaHigh = currentCandle.HighBid - previousCandle.HighBid;
                var deltaLow = previousCandle.LowBid - currentCandle.LowBid;

                decimal dmPlus = 0;
                decimal dmMinus = 0;

                if (deltaHigh > 0 && deltaHigh > deltaLow)
                {
                    dmPlus = deltaHigh;
                }
                else if (deltaLow > 0 && deltaLow > deltaHigh)
                {
                    dmMinus = deltaLow;
                }

                dmPlusList.Add(dmPlus);
                dmMinusList.Add(dmMinus);

                var trueRange =
                    (new List<decimal>
                    {
                        Math.Abs(currentCandle.HighBid - currentCandle.LowBid),
                        Math.Abs(currentCandle.HighBid - previousCandle.CloseBid),
                        Math.Abs(previousCandle.CloseBid - currentCandle.LowBid)
                    }).Max();

                trueRangeList.Add(trueRange);

                currentCandleNode = previousCandleNode;
                previousCandleNode = currentCandleNode.Previous;

                period++;
            }

            return null;
        }
    }
}