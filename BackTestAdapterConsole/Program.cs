namespace BackTestAdapterConsole
{
    using System;
    using System.Cobra;
    using System.Collections.Generic;
    using System.Linq;

    using BrokerAdapter.BackTest;
    using BrokerAdapter.Oanda;

    using TechnicalIndicators;

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
            var accountKey = "87f512402de68d8d811012662ad1c6a3-ded8ade36d14d6e042615865f07ccc66";
            var ratesProvider = new OandaAdapter("https://api-fxpractice.oanda.com/v1/",
                "https://api-fxpractice.oanda.com/v1/",
                "https://stream-fxpractice.oanda.com/v1/",
                "https://stream-fxpractice.oanda.com/v1/",
                "https://api-fxpractice.oanda.com/labs/v1/",
                accountKey);

            var candles = ratesProvider.GetLastCandles(Instrument, periodInMinutes, 500).ToList();
            var backTestAdapter = new BackTestAdapter();


            var ticks = ratesProvider.GetLastCandles(Instrument, 1, 4999).ToList();

            var system = new Cobra(new Adx(14),
                new Ema(12),
                new Ema(12),
                new Sma(72),
                new Sma(72),
                new SimpleDateProvider(),
                Instrument,
                periodInMinutes,
                backTestAdapter,
                ratesProvider,
                0,
                true);

            var initialCandles = candles.Take(72).ToList();
            system.AddCandles(initialCandles);
            foreach (var tick in ticks.Where(x=> x.Timestamp >= initialCandles.Last().Timestamp))
            {              
                var candle = candles.FirstOrDefault(x => tick.Timestamp >= x.Timestamp && tick.Timestamp < x.Timestamp.AddMinutes(periodInMinutes));
                if (candle != null)
                {
                    system.AddCandles(new List<Candle> {candle});
                }
                system.CheckRate(new Rate() {Bid = tick.FullRange/2.0m + 0.0001m, Ask = tick.FullRange / 2.0m - 0.0001m, Instrument = Instrument, Time = tick.Timestamp});
            }

            Console.ReadKey();
        }

        #endregion
    }
}