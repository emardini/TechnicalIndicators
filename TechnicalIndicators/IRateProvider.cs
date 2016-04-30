namespace TechnicalIndicators
{
    using System;
    using System.Collections.Generic;

    public interface  IRateProvider
    {
        Rate GetRate(string instrument);

        Candle GetLastCandle(string instrument, int periodInMinutes, DateTime? endDateTime=null);

        bool IsInstrumentHalted(string instrument);

        IEnumerable<Candle> GetLastCandles(string instrument, int periodInMinutes, int nbOfCandles, DateTime? endDateTime=null);
    }
}
