namespace CobraGBPUSD15MI
{
    using System;
    using System.Cobra;
    using System.Diagnostics;

    using BrokerAdapter.Oanda;

    using Microsoft.Azure.WebJobs;

    using Ninject;

    using TechnicalIndicators;

    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    internal class Program
    {
        #region Methods

        private static void Main()
        {
            var container = new StandardKernel();

            var adapter = new OandaAdapter("https://api-fxpractice.oanda.com/v1/",
              "https://api-fxpractice.oanda.com/v1/",
              "https://stream-fxpractice.oanda.com/v1/",
              "https://stream-fxpractice.oanda.com/v1/",
              "https://api-fxpractice.oanda.com/labs/v1/",
              "e304a4993098ea24bd717ab8450db9ed-497a48f0af517ebcb7dd6cc93dae4f49");

            container.Bind<Cobra>()
                .ToConstant(new Cobra(new Adx(14),
                    new Ema(12),
                    new Ema(12),
                    new Sma(72),
                    new Sma(72),
                    new SimpleDateProvider(),
                    "GBP_USD",
                    15,
                    adapter,
                    adapter,
                    7733031))
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