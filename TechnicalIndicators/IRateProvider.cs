namespace TechnicalIndicators
{
    using System;

    public interface  IRateProvider
    {
        Rate GetRate(string instrument);

        Candle GetLastCandle(string instrument, int periodInMinutes, DateTime? endDateTime=null);
    }
}
