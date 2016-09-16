namespace TechnicalIndicators
{
    using System;
    using System.Collections.Generic;

    public interface ITradingAdapter
    {
        bool HasOpenOrder(string accountId);

        bool HasOpenTrade(string accountId);

        Trade GetOpenTrade(string accountId);

        void CloseTrade(string accountId, long tradeId);

        void PlaceOrder(Order order);


        void UpdateTrade(Trade updatedTrade);

        AccountInformation GetAccountInformation(string accountId);

        IEnumerable<Candle> GetLastCandles(string p1, int p2, int p3, DateTime? endDateTime=null);
    }
}