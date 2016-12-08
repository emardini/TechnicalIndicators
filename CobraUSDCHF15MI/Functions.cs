namespace CobraUSDCHF15MI
{
    using System;
    using System.Cobra;
    using System.Diagnostics;

    using Microsoft.Azure.WebJobs;
    using TechnicalIndicators;

    public class Functions
    {
        #region Fields

        private readonly Cobra tradingSystem;
        private readonly IRateProvider rateProvider;

        public Functions(Cobra tradingSystem, IRateProvider rateProvider)
        {
            if (tradingSystem == null)
            {
                throw new ArgumentNullException("system");
            }

            this.tradingSystem = tradingSystem;
            this.rateProvider = rateProvider;
        }

        #endregion

        [Singleton(Mode = SingletonMode.Listener)]
        #region Public Methods and Operators
        public void CheckRatesCobra([TimerTrigger("*/1 * * * * MON,TUE,WED,THU,FRI", RunOnStartup = true)] TimerInfo timer)
        {
            try
            {
                if (this.rateProvider.IsInstrumentHalted(tradingSystem.Instrument))
                {
                    //If instrument is halted, there is nothing that can be done
                    Trace.TraceInformation("Instrument {0} halted", tradingSystem.Instrument);
                    return;
                }

                var newRate = this.rateProvider.GetRate(tradingSystem.Instrument);
                this.tradingSystem.CheckRate(newRate);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error at {this.tradingSystem.Instrument}: {ex}");
            }          
        }

        #endregion
    }
}