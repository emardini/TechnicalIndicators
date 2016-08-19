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
            const int periodInMinutes = 10;
            var accountKey = "e304a4993098ea24bd717ab8450db9ed-497a48f0af517ebcb7dd6cc93dae4f49";
            var ratesProvider = new OandaAdapter("https://api-fxpractice.oanda.com/v1/",
                "https://api-fxpractice.oanda.com/v1/",
                "https://stream-fxpractice.oanda.com/v1/",
                "https://stream-fxpractice.oanda.com/v1/",
                "https://api-fxpractice.oanda.com/labs/v1/",
                accountKey);

            var candles = ratesProvider.GetLastCandles(Instrument, periodInMinutes, 1000).ToList();
            var backTestAdapter = new HistoricalBackTestAdapter("https://api-fxpractice.oanda.com/v1/",
                "https://api-fxpractice.oanda.com/v1/",
                "https://stream-fxpractice.oanda.com/v1/",
                "https://stream-fxpractice.oanda.com/v1/",
                "https://api-fxpractice.oanda.com/labs/v1/",
                accountKey,
                5,
                candles,
                candles.OrderBy(x => x.Timestamp).First().Timestamp);

            backTestAdapter.Reset();
            var system = new Cobra(new Adx(14),
                new Ema(12),
                new Ema(12),
                new Sma(72),
                new Sma(72),
                new SimpleDateProvider(),
                Instrument,
                periodInMinutes,
                backTestAdapter,
                backTestAdapter,
                5027596,
                true);

            for (var i = 0; i < candles.Count() * 5; i++)
            {
                system.CheckRate();
            }

            Console.ReadKey();
        }

        #endregion
    }
}