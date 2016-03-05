namespace TechnicalIndicators
{

    public interface  IRateProvider
    {
        Rate GetRate(string instrument);

        Candle GetLastCandle(string instrument, int periodInMinutes);
    }
}
