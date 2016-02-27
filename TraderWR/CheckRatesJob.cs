namespace TraderWR
{
    using System;
    using System.Cobra;
    using System.Diagnostics;

    using Quartz;

    using TechnicalIndicators;

    [DisallowConcurrentExecution]
    public class CheckRatesJob : IJob
    {
        private readonly Cobra tradingSystem;

        private readonly IRateProvider rateProvider;

        public CheckRatesJob(Cobra tradingSystem, IRateProvider rateProvider)
        {
            if (tradingSystem == null)
            {
                throw new ArgumentNullException("tradingSystem");
            }

            if (rateProvider == null)
            {
                throw new ArgumentNullException("rateProvider");
            }

            this.tradingSystem = tradingSystem;
            this.rateProvider = rateProvider;
        }      

        #region Public Methods and Operators

        public void Execute(IJobExecutionContext context)
        {
            Trace.TraceInformation("Trade Job {0} - Agent {1}", DateTime.Now, tradingSystem.Id);
            var rate = rateProvider.GetRate("EUR_USD");
            this.tradingSystem.CheckRate(rate);
        }

        #endregion
    }
}