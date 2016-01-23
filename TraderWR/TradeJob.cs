namespace TraderWR
{
    using System;
    using System.Cobra;
    using System.Diagnostics;

    using Quartz;

    using TechnicalIndicators;

    public class TradeJob : IJob
    {
        private readonly Cobra tradingSystem;

        public TradeJob(Cobra tradingSystem)
        {
            if (tradingSystem == null)
            {
                throw new ArgumentNullException("tradingSystem");
            }

            this.tradingSystem = tradingSystem;
        }

        #region Fields

        //private readonly Cobra tradingSystem = new Cobra(new Adx(14), new List<Candle>(), new Ema(12), new Ema(12), new Sma(72), new Sma(72));

        #endregion

        #region Public Methods and Operators

        public void Execute(IJobExecutionContext context)
        {
            Trace.TraceInformation("Trade Job {0} - Agent {1}", DateTime.Now, tradingSystem.Id);
            this.tradingSystem.CheckRate(new Rate { Ask = 10.1m, Bid = 10.2m, Instrument = "EURUSD", Time = DateTime.Now });
        }

        #endregion
    }
}