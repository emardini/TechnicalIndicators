namespace TechnicalIndicators
{
    using System;
    using System.Collections.Generic;

    public interface ITradingAdapter
    {
        bool HasOpenOrder(int accountId);

        bool HasOpenTrade(int accountId);

        Trade GetOpenTrade(int accountId);

        void CloseTrade(int accountId, long tradeId);

        void PlaceOrder(Order order);


        void UpdateTrade(Trade updatedTrade);

        AccountInformation GetAccountInformation(int accountId);

        IEnumerable<Candle> GetLastCandles(string p1, int p2, int p3, DateTime? endDateTime=null);
    }
}