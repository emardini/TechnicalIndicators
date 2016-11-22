namespace CobraUSDCHF15MI
{
    using System;
    using System.Cobra;
    using System.Diagnostics;

    using BrokerAdapter.Oanda;

    using Microsoft.Azure.WebJobs;

    using Ninject;

    using TechnicalIndicators;
    using System.Configuration;
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

            container.Bind<Cobra>()
                .ToConstant(new Cobra(new Adx(14),
                    new Ema(12),
                    new Ema(12),
                    new Sma(72),
                    new Sma(72),
                    new SimpleDateProvider(),
                    "GBP_EUR",
                    15,
                    adapter,
                    adapter,
                    account))
                .InSingletonScope();

            container.Bind<IRateProvider>()
                .ToConstant(adapter)
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