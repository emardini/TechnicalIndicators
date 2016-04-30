namespace TraderWR
{
    using System;
    using System.Cobra;
    using System.Diagnostics;

    using Quartz;

    [DisallowConcurrentExecution]
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
            try
            {               
                this.tradingSystem.CheckRate();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        #endregion
    }
}