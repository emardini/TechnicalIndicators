namespace TraderWR
{
    using System;
    using System.Cobra;
    using System.Diagnostics;

    using Quartz;

    using TechnicalIndicators;

    public class CheckRatesJob : IJob
    {
        private readonly Cobra tradingSystem;

        public CheckRatesJob(Cobra tradingSystem)
        {
            if (tradingSystem == null)
            {
                throw new ArgumentNullException("tradingSystem");
            }

            this.tradingSystem = tradingSystem;
        }      

        #region Public Methods and Operators

        public void Execute(IJobExecutionContext context)
        {
            Trace.TraceInformation("Trade Job {0} - Agent {1}", DateTime.Now, tradingSystem.Id);
            this.tradingSystem.CheckRate(new Rate { Ask = 10.1m, Bid = 10.2m, Instrument = "EURUSD", Time = DateTime.Now });
        }

        #endregion
    }
}