namespace TechnicalIndicators
{
    using System.Collections.Generic;

    public class InstrumentHistory
    {
        public InstrumentHistory()
        {
            this.Candles = new List<BidAskCandle>();
        }

        public string Instrument { get; set; }

        public string Granularity { get; set; }

        public IEnumerable<BidAskCandle> Candles { get; set; }
    }
}