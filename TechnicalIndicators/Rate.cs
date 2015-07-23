namespace TechnicalIndicators
{
    using System;

    public class Rate
    {
        public decimal Bid { get; set; }

        public decimal Ask { get; set; }

        public DateTime Time { get; set; }

        public string Instrument { get; set; }
    }
}