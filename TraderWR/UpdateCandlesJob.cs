namespace TraderWR
{
    using System;
    using System.Cobra;
    using System.Diagnostics;

    using Quartz;

    using TechnicalIndicators;

    public class UpdateCandlesJob : IJob
    {
        private readonly Cobra tradingSystem;

        private readonly IRateProvider rateProvider;


        public UpdateCandlesJob(Cobra tradingSystem, IRateProvider rateProvider)
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
            Trace.TraceInformation("UpdateCandles Job {0} - Agent {1}", DateTime.Now, tradingSystem.Id);
            var candle = rateProvider.GetLastCandle(tradingSystem.Instrument, tradingSystem.PeriodInMinutes, 5027596);
            this.tradingSystem.AddCandle(candle);
        }

        #endregion
    }
}