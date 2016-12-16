namespace BackTestAdapterConsole
{
    using System;
    using System.Cobra;
    using System.Collections.Generic;
    using System.Linq;

    using BrokerAdapter.BackTest;
    using BrokerAdapter.Oanda;

    using TechnicalIndicators;
    using System.Data.Entity;

    public class Program
    {
        #region Public Methods and Operators

        public static List<T> ToEmptyItemList<T>(T that)
        {
            var list = new List<T> { that };
            list.Clear();

            return list;
        }

        #endregion

        #region Methods

        private static void Main(string[] args)
        {
            const string Instrument = "EUR_USD";
            const int periodInMinutes = 15;
            //var accountKey = "87f512402de68d8d811012662ad1c6a3-ded8ade36d14d6e042615865f07ccc66";
            //var ratesProvider = new OandaAdapter("https://api-fxpractice.oanda.com/v1/",
            //    "https://api-fxpractice.oanda.com/v1/",
            //    "https://stream-fxpractice.oanda.com/v1/",
            //    "https://stream-fxpractice.oanda.com/v1/",
            //    "https://api-fxpractice.oanda.com/labs/v1/",
            //    accountKey);

            //var candles = ratesProvider.GetLastCandles(Instrument, periodInMinutes, 500).ToList();
            var backTestAdapter = new BackTestAdapter();

            var startDate = new DateTime(2016, 10, 06,  19, 45, 0, 0);
            var startDateForInitialCandles = startDate.AddMinutes(-15 * 72);
            var candlesContext = new AskBidCandleContext();
            var a =
                candlesContext.AskBidCandles
                .Where(x => x.UTCTickDateTime >= startDateForInitialCandles && x.UTCTickDateTime < startDate)
                .GroupBy(cd =>  DbFunctions.AddMinutes(
                        DbFunctions.AddSeconds(cd.UTCTickDateTime, -cd.UTCTickDateTime.Second),
                        -cd.UTCTickDateTime.Minute % 15)
                )
                .Select(g => new
                {
                    UTCTickDateTime = g.Key,
                    CloseCandle = g.OrderByDescending(x => x.UTCTickDateTime).FirstOrDefault(),
                    OpenCandle = g.OrderBy(x => x.UTCTickDateTime).FirstOrDefault(),
                    Ticks = g.Sum(x => x.Ticks),
                    HighBid = g.Max(x => x.HighBid),
                    HighAsk = g.Max(x => x.HighAsk),
                    LowBid = g.Max(x => x.LowBid),
                    LowAsk = g.Max(x => x.LowAsk),
                    NbInnerCandles = g.Count()
                }).
                 OrderBy(x => x.UTCTickDateTime)
                .Where(x => x.UTCTickDateTime != null && x.OpenCandle != null && x.CloseCandle != null);

                var c = a.ToList()
                .Select(x=> new AskBidCandle
                {
                    UTCTickDateTime = x.UTCTickDateTime.GetValueOrDefault(),
                    OpenBid = x.OpenCandle != null ? x.OpenCandle.OpenBid : 0,
                    OpenAsk = x.OpenCandle != null ? x.OpenCandle.OpenAsk : 0,
                    CloseBid = x.OpenCandle != null ? x.CloseCandle.CloseBid : 0,
                    CloseAsk = x.OpenCandle != null ? x.CloseCandle.CloseAsk : 0,
                    Ticks = x.Ticks,
                    HighBid = x.HighBid,
                    HighAsk = x.HighAsk,
                    LowBid = x.LowBid,
                    LowAsk = x.LowAsk,
                });


            var a1 = c.Skip(1).FirstOrDefault();

           
            //.Select(g => new AskBidCandle
            //{
            //    UTCTickDateTime = g.Key.Date.GetValueOrDefault(),
            //    CloseAsk = (g.OrderByDescending(x=> x.UTCTickDateTime).FirstOrDefault()?.CloseAsk).GetValueOrDefault(),                
            //    CloseBid = (g.OrderByDescending(x => x.UTCTickDateTime).FirstOrDefault()?.CloseBid).GetValueOrDefault(),
            //    Ticks = g.Sum(x => x.Ticks)


            //});


            var system = new Cobra(new Adx(14),
                new Ema(12),
                new Ema(12),
                new Sma(72),
                new Sma(72),
                new SimpleDateProvider(),
                Instrument,
                periodInMinutes,
                backTestAdapter,
                null,
                0,
                true);

            //var initialCandles = candles.Take(72).ToList();
            //system.AddCandles(initialCandles);
            //foreach (var tick in ticks.Where(x=> x.Timestamp >= initialCandles.Last().Timestamp))
            //{              
            //    var candle = candles.FirstOrDefault(x => tick.Timestamp >= x.Timestamp && tick.Timestamp < x.Timestamp.AddMinutes(periodInMinutes));
            //    if (candle != null)
            //    {
            //        system.AddCandles(new List<Candle> {candle});
            //    }
            //    system.CheckRate(new Rate() {Bid = tick.FullRange/2.0m + 0.0001m, Ask = tick.FullRange / 2.0m - 0.0001m, Instrument = Instrument, Time = tick.Timestamp});
            //}

            Console.ReadKey();
        }

        #endregion
    }
}