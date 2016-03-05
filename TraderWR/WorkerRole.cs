namespace TraderWR
{
    using System.Cobra;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using BrokerAdapter.Oanda;

    using Ninject;
    using Ninject.Extensions.Azure;

    using Quartz;

    using TechnicalIndicators;

    public class WorkerRole : NinjectRoleEntryPoint
    {
        #region Fields

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private IKernel kernel;

        private IScheduler scheduler;

        #endregion

        #region Public Methods and Operators

        public override void Run()
        {
            Trace.TraceInformation("Company.Services.Report is running");

            try
            {
                RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        #endregion

        #region Methods

        protected override IKernel CreateKernel()
        {
            var adapter = new OandaAdapter("https://api-fxpractice.oanda.com/v1/",
                "https://api-fxpractice.oanda.com/v1/",
                "https://stream-fxpractice.oanda.com/v1/",
                "https://stream-fxpractice.oanda.com/v1/",
                "https://api-fxpractice.oanda.com/labs/v1/",
                "XXXXXXXXXXXXXX");

            this.kernel = new StandardKernel();
            this.kernel.Bind<Cobra>()
                .ToConstant(new Cobra(new Adx(14), new List<Candle>(), new Ema(12), new Ema(12), new Sma(72), new Sma(72), new SimpleDateProvider(), "EUR_USD", 10, adapter, "0000000"))
                .InSingletonScope();

            this.kernel.Bind<IRateProvider>()
              .ToConstant(adapter)
              .InSingletonScope();

            return this.kernel;
        }

        protected override bool OnRoleStarted()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            var result = base.OnRoleStarted();

            Trace.TraceInformation("Company.Services.Report has been started");
            this.ConfigureScheduler();

            return result;
        }

        protected override void OnRoleStopped()
        {
            Trace.TraceInformation("Company.Services.Report is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnRoleStopped();

            Trace.TraceInformation("Company.Services.Report has stopped");
        }

        private static async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000, cancellationToken);
            }
        }

        private void ConfigureScheduler()
        {
            this.scheduler = this.kernel.Get<IScheduler>();

            // define the job and tie it to our WorkerJob class
            var jobRate = JobBuilder.Create<CheckRatesJob>().Build();
            var jobCandle = JobBuilder.Create<UpdateCandlesJob>().Build();

            var ratesTrigger = TriggerBuilder.Create()
                .StartNow() 
                .WithDescription("Rates")                
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(5)
                    .RepeatForever())
                .Build();

            var candlesTrigger = TriggerBuilder.Create()
              .StartNow() 
              .WithDescription("Candles")
              .WithSimpleSchedule(x => x
                  .WithIntervalInSeconds(10)
                  .RepeatForever())
              .Build();

            // Tell quartz to schedule the job using our trigger
            this.scheduler.ScheduleJob(jobRate, ratesTrigger);
            this.scheduler.ScheduleJob(jobCandle, candlesTrigger);

            this.scheduler.Start();
        }

        #endregion
    }
}