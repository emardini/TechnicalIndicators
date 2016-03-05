namespace TechnicalIndicators
{
    public interface ITradingAdapter
    {
        bool HasOpenOrder(string accountId);

        bool HasOpenTrade(string accountId);
    }
}