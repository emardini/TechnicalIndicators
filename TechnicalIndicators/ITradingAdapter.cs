namespace TechnicalIndicators
{
    public interface ITradingAdapter
    {
        bool HasOpenOrder(int accountId);

        bool HasOpenTrade(int accountId);

        Trade GetOpenTrade(int accountId);

        void CloseTrade(int accountId, long tradeId);
    }
}