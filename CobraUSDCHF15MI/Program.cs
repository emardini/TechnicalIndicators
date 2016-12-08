namespace CobraUSDCHF15MI
{
    using System;
    using System.Cobra;
    using System.Collections.Generic;
    using System.Diagnostics;

    using BrokerAdapter.Oanda;

    using Microsoft.Azure.WebJobs;

    using Ninject;

    using TechnicalIndicators;
    using System.Configuration;
    using System.Linq;

    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    internal class Program
    {
        #region Methods

        private static void Main()
        {
            var container = new StandardKernel();

            var token = ConfigurationManager.AppSettings["TOKEN"];
            var account = ConfigurationManager.AppSettings["ACCOUNT_USD_CHF"].SafeParseInt().GetValueOrDefault();

            var adapter = new OandaAdapter("https://api-fxpractice.oanda.com/v1/",
              "https://api-fxpractice.oanda.com/v1/",
              "https://stream-fxpractice.oanda.com/v1/",
              "https://stream-fxpractice.oanda.com/v1/",
              "https://api-fxpractice.oanda.com/labs/v1/",
              token);

            var instrument = "EUR_GBP";
            var periodInMinutes = 15;
            var minNumberOfCandles = 72;
            var lastCandles = adapter.GetLastCandles(instrument,
                   periodInMinutes,
                   minNumberOfCandles).ToList();
            if (lastCandles.Count < minNumberOfCandles)
            {
                Trace.TraceWarning("Not enough candles to check trading, positions can still be closed");
                lastCandles = new List<Candle>();
            }

            container.Bind<IRateProvider>()
              .ToConstant(adapter)
              .InSingletonScope();

            container.Bind<Cobra>()
                .ToConstant(new Cobra(new Adx(14),
                    new Ema(12),
                    new Ema(12),
                    new Sma(72),
                    new Sma(72),
                    new SimpleDateProvider(),
                    instrument,
                    periodInMinutes,
                    adapter,
                    adapter,
                    account))
                .InSingletonScope();      
         
            var test = container.TryGet<Cobra>();
            if (test == null)
            {
                throw new Exception("Unable to build Forex System");
            }

            var config = new JobHostConfiguration
            {
                JobActivator = new MyActivator(container)
            };
            config.Tracing.ConsoleLevel = TraceLevel.Verbose;
            config.UseTimers();

            var host = new JobHost(config);
            host.RunAndBlock();
        }

        #endregion
    }
}