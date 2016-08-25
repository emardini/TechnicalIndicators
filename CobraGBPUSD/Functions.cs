namespace CobraGBPUSD
{
    using System;
    using System.Cobra;
    using System.Diagnostics;

    using Microsoft.Azure.WebJobs;

    public class Functions
    {
        #region Fields

        private readonly Cobra tradingSystem;

        public Functions(Cobra tradingSystem)
        {
            if (tradingSystem == null)
            {
                throw new ArgumentNullException("system");
            }

            this.tradingSystem = tradingSystem;
        }

        #endregion

        [Singleton(Mode = SingletonMode.Listener)]
        #region Public Methods and Operators
        public void CheckRatesCobraGBPUSD([TimerTrigger("0 */1 * * * MON,TUE,WED,THU", RunOnStartup = true)] TimerInfo timer)
        {
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